using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class GameLogic
{
    public event EventHandler<Dictionary<GameDataType, object>> UpdateHandler;
    private ReaderWriterLock _readerWriterLock = new();
    private HashSet<GameDataType> GameDataTypes { get; set; }

    public User User { get; private set; }
    public WorkerDictHelper<Cargo> CargoMaster { get; private set; }
    public WorkerDictHelper<Train> TrainMaster { get; private set; }
    public WorkerDictHelper<Station> StationMaster { get; private set; }
    public StationReacher StationReacher { get; private set; }
    private TrackMaster TrackMaster { get; set; }
    private WorkerDictHelper<CargoModel> CargoCatalog { get; set; }
    private WorkerDictHelper<TrainModel> TrainCatalog { get; set; }

    public GameLogic()
    {
        GameDataTypes = new();

        User = new("", 0, 0, new());
        CargoMaster = new();
        TrainMaster = new();
        StationMaster = new();
        StationReacher = new(StationMaster);
        TrackMaster = new();
        CargoCatalog = new();
        TrainCatalog = new();

#if UNITY_EDITOR
        GenerateCargoModels();
#endif
    }

    public void SetTrainUnityStats(
        Guid train,
        float speed,
        UnityEngine.Vector3 position,
        UnityEngine.Quaternion rotation,
        TrainDirection direction)
    {
        TrainMaster.AcquireWriterLock();
        Train trainObject = TrainMaster.GetObject(train);
        trainObject.Attribute.SetUnityStats(speed, position, rotation, direction);
        TrainMaster.ReleaseWriterLock();
    }
    public Train GetTrainRefByPosition(UnityEngine.Vector3 position)
    {
        Train train = default;
        HashSet<Guid> trains = TrainMaster.GetAll();
        foreach (var guid in trains)
        {
            Train trainObject = TrainMaster.GetRef(guid);
            if (trainObject.Attribute.Position.Equals(position))
            {
                train = trainObject;
                break;
            }
        }
        return train;
    }
    public void OnTrainArrival(Guid train)
    {
        Train trainRef = TrainMaster.GetRef(train);

        Guid station = trainRef.TravelPlan.DestinationStation;
        if (station == Guid.Empty) return; // when train is just initialised

        CurrencyManager userCurrencyManager = User.CurrencyManager;

        StationMaster.AcquireWriterLock();
        StationMaster.GetObject(station).TrainHelper.Add(train);
        StationMaster.ReleaseWriterLock();

        HashSet<Guid> cargoCollection = trainRef.CargoHelper.GetAll();
        foreach (Guid cargo in cargoCollection)
        {
            Cargo cargoRef = CargoMaster.GetRef(cargo);
            if (!cargoRef.TravelPlan.HasArrived(station)) continue;

            userCurrencyManager.AddCurrencyManager(cargoRef.CurrencyManager);
            RemoveCargoFromTrain(train, cargo);
            CargoMaster.Remove(cargo);
        }
       
        GameDataTypes.Add(GameDataType.User);
        GameDataTypes.Add(GameDataType.CargoMaster);
    }
    public TrainDepartStatus OnTrainDeparture(Guid train)
    {
        Train trainObject = TrainMaster.GetObject(train);

        TrainAttribute trainAttribute = trainObject.Attribute;
        if (!trainAttribute.BurnFuel())
            return TrainDepartStatus.OutOfFuel;
        if (!trainAttribute.DurabilityWear())
            return TrainDepartStatus.OutOfDurability;

        Guid sourceStation = trainObject.TravelPlan.SourceStation;
        if (sourceStation == Guid.Empty) return TrainDepartStatus.Error;
        StationMaster.GetObject(sourceStation).TrainHelper.Remove(train);

        GameDataTypes.Add(GameDataType.TrainMaster);
        GameDataTypes.Add(GameDataType.StationMaster);
        return TrainDepartStatus.Success;
    }
    public void SetTrainTravelPlan(Guid train, Guid sourceStation, Guid destinationStation)
    {
        Train trainObject = TrainMaster.GetObject(train);
        trainObject.TravelPlan.SetSourceStation(sourceStation);
        trainObject.TravelPlan.SetDestinationStation(destinationStation);

        GameDataTypes.Add(GameDataType.TrainMaster);
    }
    public void ReplenishTrainFuelAndDurability(Guid train)
    {
        TrainAttribute trainAttribute = TrainMaster.GetObject(train).Attribute;
        trainAttribute.Refuel();
        trainAttribute.DurabilityRepair();
    }
    public bool AddCargoToTrain(Guid train, Guid cargo)
    {
        Train trainObject = TrainMaster.GetObject(train);
        if (trainObject.Attribute.IsCapacityFull()) return false;

        trainObject.CargoHelper.Add(cargo);
        trainObject.Attribute.AddToCapacity();
        CargoMaster.GetObject(cargo).SetCargoAssoc(CargoAssociation.TRAIN);

        GameDataTypes.Add(GameDataType.TrainMaster);
        GameDataTypes.Add(GameDataType.CargoMaster);

        return true;
    }
    public void RemoveCargoFromTrain(Guid train, Guid cargo)
    {
        Train trainObject = TrainMaster.GetObject(train);
        trainObject.CargoHelper.Remove(cargo);
        trainObject.Attribute.RemoveFromCapacity();

        GameDataTypes.Add(GameDataType.TrainMaster);
        GameDataTypes.Add(GameDataType.CargoMaster);
    }
    public Guid InitTrain(
        string trainName,
        double maxSpeed,
        UnityEngine.Vector3 position,
        UnityEngine.Quaternion rotation,
        TrainDirection direction)
    {
        TrainAttribute attribute = new(
            new(0, 4, 0, 0),
            new(0.0, 100.0, 100.0, 5.0),
            new(0.0, 100.0, 100.0, 5.0),
            new(0.0, maxSpeed, 0.0, 0.0),
            position,
            rotation,
            direction);
        Train train = new(
            trainName,
            TrainType.Steam,
            attribute,
            new());
        TrainMaster.Add(train);

        GameDataTypes.Add(GameDataType.TrainMaster);

        return train.Guid;
    }

    public Station GetStationRefByPosition(UnityEngine.Vector3 position)
    {
        Station station = default;
        HashSet<Guid> stations = StationMaster.GetAll();
        foreach (var guid in stations)
        {
            Station stationObject = StationMaster.GetRef(guid);
            if (stationObject.Attribute.Position.Equals(position))
            {
                station = stationObject;
                break;
            }
        }
        return station;
    }
    public void AddStationLinks(Guid station1, Guid station2)
    {
        StationMaster.GetObject(station1).StationHelper.Add(station2);
        StationMaster.GetObject(station2).StationHelper.Add(station1);
        StationReacher = new(StationMaster); // TODO: optimise this in the future

        GameDataTypes.Add(GameDataType.StationMaster);
        GameDataTypes.Add(GameDataType.StationReacher);
    }
    public void RemoveStationLinks(Guid station1, Guid station2)
    {
        StationMaster.GetObject(station1).StationHelper.Remove(station2);
        StationMaster.GetObject(station2).StationHelper.Remove(station1);
        StationReacher.UnlinkStations(station1, station2);

        GameDataTypes.Add(GameDataType.StationMaster);
        GameDataTypes.Add(GameDataType.StationReacher);
    }
    public Track GetTrackInfo(int srcStationNum, int destStationNum)
    {
        return TrackMaster.GetTrack(srcStationNum, destStationNum);
    }
    public void AddRandomCargoToStation(Guid station, int numOfRandomCargo)
    {
        List<Guid> subStations = StationReacher.ReacherDict.GetObject(station).GetAll().ToList();
        Random rand = new();

        for (int i = 0; i < numOfRandomCargo; i++)
        {
            CargoModel cargoModel = GetRandomCargoModel();
            cargoModel.Randomise();
            Guid destination = subStations[rand.Next(subStations.Count - 1)];

            Cargo cargo = new(cargoModel, station, destination);
            CargoMaster.Add(cargo);
            AddCargoToStation(station, cargo.Guid);
        }

        GameDataTypes.Add(GameDataType.CargoMaster);
    }
    public bool AddCargoToStation(Guid station, Guid cargo)
    {
        Station stationObject = StationMaster.GetObject(station);
        Cargo cargoObject = CargoMaster.GetObject(cargo);
        
        if (!cargoObject.TravelPlan.IsAtSource(station))
        {
            if (stationObject.Attribute.IsYardFull())
                return false;
            stationObject.Attribute.AddToYard();
            cargoObject.SetCargoAssoc(CargoAssociation.YARD);
        }
        else 
            cargoObject.SetCargoAssoc(CargoAssociation.STATION);

        stationObject.CargoHelper.Add(cargo);

        GameDataTypes.Add(GameDataType.StationMaster);
        GameDataTypes.Add(GameDataType.CargoMaster);
        return true;
    }
    public void RemoveCargoFromStation(Guid station, Guid cargo)
    {
        Station stationObject = StationMaster.GetObject(station);
        stationObject.CargoHelper.Remove(cargo);

        Cargo cargoObject = CargoMaster.GetObject(cargo);
        if (!cargoObject.TravelPlan.IsAtSource(station))
            stationObject.Attribute.RemoveFromYard();

        GameDataTypes.Add(GameDataType.StationMaster);
        GameDataTypes.Add(GameDataType.CargoMaster);
    }
    public Guid InitStation(string stationName, UnityEngine.Vector3 position)
    {
        StationAttribute stationAttribute = new(
            new(0, 5, 0, 0),
            position);
        Station station = new(
                stationName,
                StationStatus.Open,
                stationAttribute,
                new(),
                new(),
                new());

        StationMaster.Add(station);
        if (station.StationHelper.Count() > 0)
        {
            StationReacher.Bfs(StationMaster);
            GameDataTypes.Add(GameDataType.StationReacher);
        }
        // TODO: Check if all stations in StationHelper exists before running Bfs

        GameDataTypes.Add(GameDataType.StationMaster);

        return station.Guid;
    }

    public CargoModel GetRandomCargoModel()
    {
        List<Guid> keys = CargoCatalog.GetAll().ToList();

        Random rand = new();
        int randomIndex = rand.Next(keys.Count - 1);

        Guid randomGuid = keys[randomIndex];
        return CargoCatalog.GetRef(randomGuid);
    }

    public void SetDataFromPlayfab(GameDataType gameDataType, object data)
    {
        switch (gameDataType)
        {
            case GameDataType.User:
                {
                    User = (User)data;
                    break;
                }
            case GameDataType.CargoMaster:
                {
                    CargoMaster = (WorkerDictHelper<Cargo>)data;
                    break;
                }
            case GameDataType.CargoCatalog:
                {
                    CargoCatalog = (WorkerDictHelper<CargoModel>)data;
                    break;
                }
            case GameDataType.TrainMaster:
                {
                    TrainMaster = (WorkerDictHelper<Train>)data;
                    break;
                }
            case GameDataType.TrainCatalog:
                {
                    TrainCatalog = (WorkerDictHelper<TrainModel>)data;
                    break;
                }
            case GameDataType.StationMaster:
                {
                    StationMaster = (WorkerDictHelper<Station>)data;
                    break;
                }
            case GameDataType.StationReacher:
                {
                    StationReacher = (StationReacher)data;
                    break;
                }
            case GameDataType.TrackMaster:
                {
                    TrackMaster = (TrackMaster)data;
                    break;
                }
        }
    }
    public void SendDataToPlayfab()
    {
#if UNITY_EDITOR
#else
        _readerWriterLock.AcquireReaderLock(Timeout.Infinite);

        Dictionary<GameDataType, object> gameDataDict = new();
        GameDataTypes.ToList().ForEach(gameDataType => 
        {
            string data = "";
            switch (gameDataType)
            {
                case GameDataType.User:
                    data = GameDataManager.Serialize(User);
                    break;
                case GameDataType.CargoMaster:
                    data = GameDataManager.Serialize(CargoMaster);
                    break;
                case GameDataType.CargoCatalog:
                    data = GameDataManager.Serialize(CargoCatalog);
                    break;
                case GameDataType.TrainMaster:
                    data = GameDataManager.Serialize(TrainMaster);
                    break;
                case GameDataType.TrainCatalog:
                    data = GameDataManager.Serialize(TrainCatalog);
                    break;
                case GameDataType.StationMaster:
                    data = GameDataManager.Serialize(StationMaster);
                    break;
                case GameDataType.StationReacher:
                    data = GameDataManager.Serialize(StationReacher);
                    break;
            }
            gameDataDict.Add(gameDataType, data);
        });
        GameDataTypes = new();
        UpdateHandler?.Invoke(this, gameDataDict);

        _readerWriterLock.ReleaseReaderLock();
#endif
    }


    /////////// QUICK FIXES ///////////  
    public void GenerateCargoModels()
    {
        Random rand = new();
        CargoType[] cargoTypes = (CargoType[])Enum.GetValues(typeof(CargoType));
        CurrencyType[] currencyTypes = (CurrencyType[])Enum.GetValues(typeof(CurrencyType));
        foreach (var cargoType in cargoTypes)
        {
            rand = new Random(Guid.NewGuid().GetHashCode());
            CurrencyManager currencyManager = new();
            CurrencyType randomType = currencyTypes[rand.Next(currencyTypes.Length)];
            double randomAmount = 0;
            switch (randomType)
            {
                case CurrencyType.Coin:
                    randomAmount = rand.Next(10, 100);
                    break;
                case CurrencyType.Note:
                    randomAmount = rand.Next(1, 5);
                    break;
                case CurrencyType.NormalCrate:
                    randomAmount = rand.Next(1, 1);
                    break;
                case CurrencyType.SpecialCrate:
                    randomAmount = rand.Next(1, 1);
                    break;
                default:
                    randomAmount = rand.Next(1, 1);
                    break;
            }
            Currency currency = new(randomType, randomAmount);
            currencyManager.AddCurrency(currency);

            CargoModel cargoModel = new(cargoType, 15, 20, currencyManager);
            CargoCatalog.Add(cargoModel);
        }

        GameDataTypes.Add(GameDataType.CargoCatalog);
    }

    public void GenerateTracks(string stationName)
    {
        if (stationName != "Station5") return;

        HashSet<Guid> guids = StationMaster.GetAll();

        Dictionary<string, Guid> stationGuids = new();
        foreach (var guid in guids)
        {
            Station station = StationMaster.GetRef(guid);
            stationGuids.Add(station.Name, station.Guid);
        }

        AddStationLinks(stationGuids["Station1"], stationGuids["Station2"]);
        AddStationLinks(stationGuids["Station2"], stationGuids["Station3"]);
        AddStationLinks(stationGuids["Station3"], stationGuids["Station4"]);
        AddStationLinks(stationGuids["Station4"], stationGuids["Station5"]);
        AddStationLinks(stationGuids["Station5"], stationGuids["Station1"]);
    }
}
