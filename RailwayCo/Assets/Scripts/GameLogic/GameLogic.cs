using System;
using System.Collections.Generic;
using System.Linq;

public class GameLogic
{
    public event EventHandler<GameDataType> UpdateHandler;

    public User User { get; private set; }
    public WorkerDictHelper<Cargo> CargoMaster { get; private set; }
    public WorkerDictHelper<Train> TrainMaster { get; private set; }
    public WorkerDictHelper<Station> StationMaster { get; private set; }
    public WorkerDictHelper<CargoModel> CargoCatalog { get; private set; }
    public WorkerDictHelper<TrainModel> TrainCatalog { get; private set; }
    public StationReacher StationReacher { get; private set; }

    public GameLogic()
    {
        User = new("", 0, 0, new());
        CargoMaster = new();
        TrainMaster = new();
        StationMaster = new();
        CargoCatalog = new();
        TrainCatalog = new();
        StationReacher = new(StationMaster);
    }

    public Train GetTrainRefByPosition(UnityEngine.Vector3 position)
    {
        Train train = default;
        HashSet<Guid> trains = TrainMaster.GetAll();
        foreach (var guid in trains)
        {
            train = TrainMaster.GetRef(guid);
            if (train.Attribute.Position.Equals(position)) break;
        }
        return train;
    }
    public void OnTrainArrival(Guid train)
    {
        Train trainRef = TrainMaster.GetRef(train);

        Guid station = trainRef.TravelPlan.DestinationStation;
        CurrencyManager userCurrencyManager = User.CurrencyManager;

        StationMaster.GetObject(station).TrainHelper.Add(train);

        HashSet<Guid> cargoCollection = trainRef.CargoHelper.GetAll();
        foreach (Guid cargo in cargoCollection)
        {
            Cargo cargoRef = CargoMaster.GetRef(cargo);
            if (!cargoRef.TravelPlan.HasArrived(station)) continue;

            userCurrencyManager.AddCurrencyManager(cargoRef.CurrencyManager);
            RemoveCargoFromTrain(train, cargo);
            CargoMaster.Remove(cargo);
        }

        SendDataToPlayfab(GameDataType.User);
        SendDataToPlayfab(GameDataType.CargoMaster);
        SendDataToPlayfab(GameDataType.TrainMaster);
    }
    public void OnTrainDeparture(Guid train, Guid sourceStation, Guid destinationStation)
    {
        // TODO: Check if train has sufficient fuel and durability

        SetTrainTravelPlan(train, sourceStation, destinationStation);
        StationMaster.GetObject(sourceStation).TrainHelper.Remove(train);

        SendDataToPlayfab(GameDataType.TrainMaster);
        SendDataToPlayfab(GameDataType.StationMaster);
    }
    public void SetTrainTravelPlan(Guid train, Guid sourceStation, Guid destinationStation)
    {
        Train trainObject = TrainMaster.GetObject(train);
        trainObject.TravelPlan.SetSourceStation(sourceStation);
        trainObject.TravelPlan.SetDestinationStation(destinationStation);
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
        SendDataToPlayfab(GameDataType.TrainMaster);

        return train.Guid;
    }
    private void AddCargoToTrain(Guid train, Guid cargo)
    {
        TrainMaster.GetObject(train).CargoHelper.Add(cargo);
        CargoMaster.GetObject(cargo).SetCargoAssoc(CargoAssociation.TRAIN);
        // TODO: Check train capacity

        SendDataToPlayfab(GameDataType.TrainMaster);
    }
    private void RemoveCargoFromTrain(Guid train, Guid cargo)
    {
        TrainMaster.GetObject(train).CargoHelper.Remove(cargo);
        CargoMaster.GetObject(cargo).SetCargoAssoc(CargoAssociation.NIL);

        SendDataToPlayfab(GameDataType.TrainMaster);
    }

    public void MoveCargoFromStationToTrain(Guid cargo, Guid station, Guid train)
    {
        RemoveCargoFromStation(station, cargo);
        AddCargoToTrain(train, cargo);
    }
    public void MoveCargoFromTrainToStation(Guid cargo, Guid station, Guid train)
    {
        RemoveCargoFromTrain(train, cargo);
        AddCargoToStation(station, cargo);
    }

    public Station GetStationRefByPosition(UnityEngine.Vector3 position)
    {
        Station station = default;
        HashSet<Guid> stations = StationMaster.GetAll();
        foreach (var guid in stations)
        {
            station = StationMaster.GetRef(guid);
            if (station.Attribute.Position.Equals(position)) break;
        }
        return station;
    }
    /// <summary> This method adds a track between 2 stations such that orientation1_orientation2 is
    /// orientation1 of station1 connected to orientation2 of station2 </summary>
    public void AddStationToStation(Guid station1, Guid station2, StationOrientation orientation)
    {
        StationOrientation station1Orientation = orientation;
        StationOrientation station2Orientation = orientation;

        switch (orientation)
        {
            case StationOrientation.Head_Head:
                {
                    break;
                }
            case StationOrientation.Tail_Tail:
                {
                    break;
                }
            case StationOrientation.Head_Tail:
                {
                    station1Orientation = StationOrientation.Head_Tail;
                    station2Orientation = StationOrientation.Tail_Head;
                    break;
                }
            case StationOrientation.Tail_Head:
                {
                    station1Orientation = StationOrientation.Tail_Head;
                    station2Orientation = StationOrientation.Head_Tail;
                    break;
                }
        }

        // Stores the orientation needed to get to destination station
        StationMaster.GetObject(station1).StationHelper.Add(station2, station1Orientation);
        StationMaster.GetObject(station2).StationHelper.Add(station1, station2Orientation);
        StationReacher = new(StationMaster); // TODO: optimise this in the future

        SendDataToPlayfab(GameDataType.StationMaster);
        SendDataToPlayfab(GameDataType.StationReacher);
    }
    public void RemoveStationFromStation(Guid station1, Guid station2)
    {
        StationMaster.GetObject(station1).StationHelper.Remove(station2);
        StationMaster.GetObject(station2).StationHelper.Remove(station1);
        StationReacher.UnlinkStations(station1, station2);

        SendDataToPlayfab(GameDataType.StationMaster);
        SendDataToPlayfab(GameDataType.StationReacher);
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

        SendDataToPlayfab(GameDataType.CargoMaster);
        SendDataToPlayfab(GameDataType.StationMaster);
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
        if (station.StationHelper.Count() > 0) StationReacher.Bfs(StationMaster);
        // TODO: Check if all stations in StationHelper exists before running Bfs

        SendDataToPlayfab(GameDataType.StationMaster);

        return station.Guid;
    }
    private void AddCargoToStation(Guid station, Guid cargo)
    {
        Station stationObject = StationMaster.GetObject(station);
        stationObject.CargoHelper.Add(cargo);

        Cargo cargoObject = CargoMaster.GetObject(cargo);
        cargoObject.SetCargoAssoc(CargoAssociation.STATION);
        if (!cargoObject.TravelPlan.IsAtSource(station))
        {
            stationObject.Attribute.AddToYard();
            cargoObject.SetCargoAssoc(CargoAssociation.YARD);
        }
    }
    private void RemoveCargoFromStation(Guid station, Guid cargo)
    {
        StationMaster.GetObject(station).CargoHelper.Remove(cargo);

        CargoMaster.GetObject(cargo).SetCargoAssoc(CargoAssociation.NIL);

        Cargo cargoRef = CargoMaster.GetRef(cargo);
        if (!cargoRef.TravelPlan.IsAtSource(station))
            StationMaster.GetObject(station).Attribute.RemoveFromYard();
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
        }
    }
    private void SendDataToPlayfab(GameDataType gameDataType)
    {
        switch (gameDataType)
        {
            case GameDataType.User:
                {
                    UpdateHandler?.Invoke(User, gameDataType);
                    break;
                }
            case GameDataType.CargoMaster:
                {
                    UpdateHandler?.Invoke(CargoMaster, gameDataType);
                    break;
                }
            case GameDataType.CargoCatalog:
                {
                    UpdateHandler?.Invoke(CargoCatalog, gameDataType);
                    break;
                }
            case GameDataType.TrainMaster:
                {
                    UpdateHandler?.Invoke(TrainMaster, gameDataType);
                    break;
                }
            case GameDataType.TrainCatalog:
                {
                    UpdateHandler?.Invoke(TrainCatalog, gameDataType);
                    break;
                }
            case GameDataType.StationMaster:
                {
                    UpdateHandler?.Invoke(StationMaster, gameDataType);
                    break;
                }
            case GameDataType.StationReacher:
                {
                    UpdateHandler?.Invoke(StationReacher, gameDataType);
                    break;
                }
        }
    }


    /////////// QUICK FIXES ///////////  
    public void GenerateRandomData()
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

        SendDataToPlayfab(GameDataType.CargoCatalog);
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

        AddStationToStation(stationGuids["Station1"], stationGuids["Station2"], StationOrientation.Head_Tail);
        AddStationToStation(stationGuids["Station2"], stationGuids["Station3"], StationOrientation.Head_Tail);
        AddStationToStation(stationGuids["Station3"], stationGuids["Station4"], StationOrientation.Head_Head);
        AddStationToStation(stationGuids["Station4"], stationGuids["Station5"], StationOrientation.Tail_Head);
        AddStationToStation(stationGuids["Station5"], stationGuids["Station1"], StationOrientation.Tail_Tail);
    }
}
