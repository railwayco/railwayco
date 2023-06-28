using System;
using System.Collections.Generic;
using System.Linq;

public class GameLogic
{
    public event EventHandler<Dictionary<GameDataType, object>> UpdateHandler;

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

        List<GameDataType> gameDataTypes = new();        
        gameDataTypes.Add(GameDataType.User);
        gameDataTypes.Add(GameDataType.CargoMaster);
        gameDataTypes.Add(GameDataType.TrainMaster);
        SendDataToPlayfab(gameDataTypes);
    }
    public void OnTrainDeparture(Guid train, Guid sourceStation, Guid destinationStation)
    {
        // TODO: Check if train has sufficient fuel and durability

        SetTrainTravelPlan(train, sourceStation, destinationStation);
        StationMaster.GetObject(sourceStation).TrainHelper.Remove(train);

        List<GameDataType> gameDataTypes = new();
        gameDataTypes.Add(GameDataType.TrainMaster);
        gameDataTypes.Add(GameDataType.StationMaster);
        SendDataToPlayfab(gameDataTypes);
    }
    public void SetTrainTravelPlan(Guid train, Guid sourceStation, Guid destinationStation)
    {
        Train trainObject = TrainMaster.GetObject(train);
        trainObject.TravelPlan.SetSourceStation(sourceStation);
        trainObject.TravelPlan.SetDestinationStation(destinationStation);
    }
    public void AddCargoToTrain(Guid train, Guid cargo)
    {
        // TODO: Change method return type to bool

        // TODO: Check and update train capacity
        TrainMaster.GetObject(train).CargoHelper.Add(cargo);
        CargoMaster.GetObject(cargo).SetCargoAssoc(CargoAssociation.TRAIN);

        List<GameDataType> gameDataTypes = new();
        gameDataTypes.Add(GameDataType.TrainMaster);
        SendDataToPlayfab(gameDataTypes);
    }
    public void RemoveCargoFromTrain(Guid train, Guid cargo)
    {
        // TODO: Update train capacity
        TrainMaster.GetObject(train).CargoHelper.Remove(cargo);
        CargoMaster.GetObject(cargo).SetCargoAssoc(CargoAssociation.NIL);

        List<GameDataType> gameDataTypes = new();
        gameDataTypes.Add(GameDataType.TrainMaster);
        SendDataToPlayfab(gameDataTypes);
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

        List<GameDataType> gameDataTypes = new();
        gameDataTypes.Add(GameDataType.TrainMaster);
        SendDataToPlayfab(gameDataTypes);

        return train.Guid;
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

        List<GameDataType> gameDataTypes = new();
        gameDataTypes.Add(GameDataType.StationMaster);
        gameDataTypes.Add(GameDataType.StationReacher);
        SendDataToPlayfab(gameDataTypes);
    }
    public void RemoveStationFromStation(Guid station1, Guid station2)
    {
        StationMaster.GetObject(station1).StationHelper.Remove(station2);
        StationMaster.GetObject(station2).StationHelper.Remove(station1);
        StationReacher.UnlinkStations(station1, station2);

        List<GameDataType> gameDataTypes = new();
        gameDataTypes.Add(GameDataType.StationMaster);
        gameDataTypes.Add(GameDataType.StationReacher);
        SendDataToPlayfab(gameDataTypes);
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

        List<GameDataType> gameDataTypes = new();
        gameDataTypes.Add(GameDataType.CargoMaster);
        gameDataTypes.Add(GameDataType.StationMaster);
        SendDataToPlayfab(gameDataTypes);
    }
    public void AddCargoToStation(Guid station, Guid cargo)
    {
        // TODO: Change method return type to bool

        Station stationObject = StationMaster.GetObject(station);
        // TODO: Check yard capacity
        stationObject.CargoHelper.Add(cargo);

        Cargo cargoObject = CargoMaster.GetObject(cargo);
        cargoObject.SetCargoAssoc(CargoAssociation.STATION);
        if (!cargoObject.TravelPlan.IsAtSource(station))
        {
            stationObject.Attribute.AddToYard();
            cargoObject.SetCargoAssoc(CargoAssociation.YARD);
        }
    }
    public void RemoveCargoFromStation(Guid station, Guid cargo)
    {
        Station stationObject = StationMaster.GetObject(station);
        stationObject.CargoHelper.Remove(cargo);

        Cargo cargoObject = CargoMaster.GetObject(cargo);
        cargoObject.SetCargoAssoc(CargoAssociation.NIL);
        if (!cargoObject.TravelPlan.IsAtSource(station))
            stationObject.Attribute.RemoveFromYard();
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

        List<GameDataType> gameDataTypes = new();
        gameDataTypes.Add(GameDataType.StationMaster);
        SendDataToPlayfab(gameDataTypes);

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
        }
    }
    private void SendDataToPlayfab(List<GameDataType> gameDataTypes)
    {
        Dictionary<GameDataType, object> gameDataDict = new();
        gameDataTypes.ForEach(gameDataType => 
        {
            if (gameDataType == GameDataType.User) gameDataDict.Add(gameDataType, User);
            else if (gameDataType == GameDataType.CargoMaster) gameDataDict.Add(gameDataType, CargoMaster);
            else if (gameDataType == GameDataType.CargoCatalog) gameDataDict.Add(gameDataType, CargoCatalog);
            else if (gameDataType == GameDataType.TrainMaster) gameDataDict.Add(gameDataType, TrainMaster);
            else if (gameDataType == GameDataType.TrainCatalog) gameDataDict.Add(gameDataType, TrainCatalog);
            else if (gameDataType == GameDataType.StationMaster) gameDataDict.Add(gameDataType, StationMaster);
            else if (gameDataType == GameDataType.StationReacher) gameDataDict.Add(gameDataType, StationReacher);
        });
        UpdateHandler?.Invoke(this, gameDataDict);
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

        List<GameDataType> gameDataTypes = new();
        gameDataTypes.Add(GameDataType.CargoCatalog);
        SendDataToPlayfab(gameDataTypes);
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
