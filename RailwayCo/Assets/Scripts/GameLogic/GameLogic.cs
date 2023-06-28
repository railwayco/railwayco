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

    public string GetUserName() => User.Name;
    public int GetUserExperiencePoints() => User.ExperiencePoint;
    public int GetUserSkillPoints() => User.SkillPoint;
    public CurrencyManager GetUserCurrencyManager() => User.CurrencyManager;
    private void AddUserExperiencePoints(int experiencePoints) => User.AddExperiencePoint(experiencePoints);
    private void AddUserSkillPoints(int skillPoints) => User.AddSkillPoint(skillPoints);
    private void RemoveUserSkillPoints(int skillPoints) => User.RemoveSkillPoint(skillPoints);

    public HashSet<Guid> GetAllCargoGuids() => CargoMaster.GetAllGuids();
    public Cargo GetCargoRef(Guid cargo) => CargoMaster.GetRef(cargo);
    private Cargo GetCargoObject(Guid cargo) => CargoMaster.GetObject(cargo);
    private void AddCargo(Cargo cargo) => CargoMaster.Add(cargo);
    private void RemoveCargo(Guid cargo) => CargoMaster.Remove(cargo);
    private void SetCargoAssociation(Guid cargo, CargoAssociation cargoAssoc)
    {
        CargoMaster.GetObject(cargo).SetCargoAssoc(cargoAssoc);
        SendDataToPlayfab(GameDataType.CargoMaster);
    }

    public Train GetTrainRefByPosition(UnityEngine.Vector3 position)
    {
        // TODO: refactor this code
        
        Train train = default;
        List<Guid> trains = GetAllTrainGuids().ToList();
        foreach (var guid in trains)
        {
            if (GetTrainAttribute(guid).Position.Equals(position))
            {
                train = GetTrainRef(guid);
                break;
            }
        }
        return train;
    }
    public void SetTrainUnityStats(
        Guid train,
        float speed,
        UnityEngine.Vector3 position,
        UnityEngine.Quaternion rotation,
        TrainDirection direction) => GetTrainAttribute(train).SetUnityStats(speed, position, rotation, direction);
    public void OnTrainArrival(Guid train)
    {
        Guid station = GetTrainDestination(train);
        CurrencyManager userCurrencyManager = GetUserCurrencyManager();

        AddTrainToStation(station, train);
        HashSet<Guid> cargoCollection = GetAllCargoGuidsFromTrain(train);
        foreach (Guid cargo in cargoCollection)
        {
            Cargo cargoRef = GetCargoRef(cargo);
            if (!cargoRef.TravelPlan.HasArrived(station)) continue;

            userCurrencyManager.AddCurrencyManager(cargoRef.CurrencyManager);
            RemoveCargoFromTrain(train, cargo);
            RemoveCargo(cargo);
        }

        SendDataToPlayfab(GameDataType.User);
        SendDataToPlayfab(GameDataType.CargoMaster);
        SendDataToPlayfab(GameDataType.TrainMaster);
    }
    public void OnTrainDeparture(Guid train, Guid sourceStation, Guid destinationStation)
    {
        // TODO: Check if train has sufficient fuel and durability

        SetTrainTravelPlan(train, sourceStation, destinationStation);
        RemoveTrainFromStation(sourceStation, train);

        SendDataToPlayfab(GameDataType.TrainMaster);
        SendDataToPlayfab(GameDataType.StationMaster);
    }
    public Guid GetTrainDestination(Guid train) => GetTrainRef(train).TravelPlan.DestinationStation;
    public void SetTrainTravelPlan(Guid train, Guid sourceStation, Guid destinationStation)
    {
        GetTrainObject(train).TravelPlan.SetSourceStation(sourceStation);
        GetTrainObject(train).TravelPlan.SetDestinationStation(destinationStation);
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

        AddTrain(train);
        SendDataToPlayfab(GameDataType.TrainMaster);

        return train.Guid;
    }
    public HashSet<Guid> GetAllCargoGuidsFromTrain(Guid train) => GetTrainRef(train).CargoHelper.GetAll();
    public HashSet<Guid> GetAllTrainGuids() => TrainMaster.GetAllGuids();
    public Train GetTrainRef(Guid train) => TrainMaster.GetRef(train);
    private Train GetTrainObject(Guid train) => TrainMaster.GetObject(train);
    private TrainAttribute GetTrainAttribute(Guid train) => TrainMaster.GetObject(train).Attribute;
    private void AddTrain(Train train) => TrainMaster.Add(train);
    private void RemoveTrain(Guid train) => TrainMaster.Remove(train);
    private void AddCargoToTrain(Guid train, Guid cargo)
    {
        GetTrainObject(train).CargoHelper.Add(cargo);
        SetCargoAssociation(cargo, CargoAssociation.TRAIN);
        // TODO: Check train capacity

        SendDataToPlayfab(GameDataType.TrainMaster);
    }
    private void RemoveCargoFromTrain(Guid train, Guid cargo)
    {
        GetTrainObject(train).CargoHelper.Remove(cargo);
        SetCargoAssociation(cargo, CargoAssociation.NIL);

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
        // TODO: refactor this code

        Station station = default;
        List<Guid> stations = GetAllStationGuids().ToList();
        foreach (var guid in stations)
        {
            if (GetStationAttribute(guid).Position.Equals(position))
            {
                station = GetStationRef(guid);
                break;
            }
        }
        return station;
    }
    public void SetStationUnityStats(
        Guid station,
        UnityEngine.Vector3 position) 
    {
        GetStationAttribute(station).SetUnityStats(position);

        SendDataToPlayfab(GameDataType.StationMaster);
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
        GetStationObject(station1).StationHelper.Add(station2, station1Orientation);
        GetStationObject(station2).StationHelper.Add(station1, station2Orientation);
        StationReacher = new(StationMaster); // TODO: optimise this in the future

        SendDataToPlayfab(GameDataType.StationMaster);
        SendDataToPlayfab(GameDataType.StationReacher);
    }
    public void RemoveStationFromStation(Guid station1, Guid station2)
    {
        GetStationObject(station1).StationHelper.Remove(station2);
        GetStationObject(station2).StationHelper.Remove(station1);
        StationReacher.UnlinkStations(station1, station2);

        SendDataToPlayfab(GameDataType.StationMaster);
        SendDataToPlayfab(GameDataType.StationReacher);
    }
    public HashSet<Guid> GetAllStationGuidsFromStation(Guid station)
    {
        return GetStationRef(station).StationHelper.GetAllGuids();
    }
    public HashSet<Guid> GetAllCargoGuidsFromStation(Guid station) => GetStationRef(station).CargoHelper.GetAll();
    public void AddRandomCargoToStation(Guid station, int numOfRandomCargo)
    {
        Station stationObject = GetStationObject(station);
        List<Guid> subStations = StationReacher.ReacherDict.GetObject(station).GetAll().ToList();
        Random rand = new();

        for (int i = 0; i < numOfRandomCargo; i++)
        {
            CargoModel cargoModel = GetRandomCargoModel();
            cargoModel.Randomise();
            Guid destination = subStations[rand.Next(subStations.Count - 1)];

            Cargo cargo = new(cargoModel, station, destination);
            AddCargo(cargo);
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
        
        AddStation(station);
        SendDataToPlayfab(GameDataType.StationMaster);

        return station.Guid;
    }
    public HashSet<Guid> GetAllStationGuids() => StationMaster.GetAllGuids();
    public Station GetStationRef(Guid station) => StationMaster.GetRef(station);
    private Station GetStationObject(Guid station) => StationMaster.GetObject(station);
    private StationAttribute GetStationAttribute(Guid station) => GetStationObject(station).Attribute;
    private void AddStation(Station station)
    {
        StationMaster.Add(station);
        if (station.StationHelper.Count() > 0) StationReacher.Bfs(StationMaster);
        // TODO: Check if all stations in StationHelper exists before running Bfs
    }
    private void RemoveStation(Guid station)
    {
        StationMaster.Remove(station);
        StationReacher.RemoveStation(station);
    }
    private void AddCargoToStation(Guid station, Guid cargo)
    {
        GetStationObject(station).CargoHelper.Add(cargo);

        Cargo cargoRef = GetCargoRef(cargo);
        if (!cargoRef.TravelPlan.IsAtSource(station))
        {
            GetStationAttribute(station).AddToYard();
            SetCargoAssociation(cargo, CargoAssociation.YARD);
        }
        else
        {
            SetCargoAssociation(cargo, CargoAssociation.STATION);
        }
    }
    private void RemoveCargoFromStation(Guid station, Guid cargo)
    {
        GetStationObject(station).CargoHelper.Remove(cargo);

        SetCargoAssociation(cargo, CargoAssociation.NIL);

        Cargo cargoRef = GetCargoRef(cargo);
        if (!cargoRef.TravelPlan.IsAtSource(station)) GetStationAttribute(station).RemoveFromYard();
    }
    private void AddTrainToStation(Guid station, Guid train) => GetStationObject(station).TrainHelper.Add(train);
    private void RemoveTrainFromStation(Guid station, Guid train) => GetStationObject(station).TrainHelper.Remove(train);

    public CargoModel GetRandomCargoModel()
    {
        List<Guid> keys = GetAllCargoModelGuids().ToList();

        Random rand = new();
        int randomIndex = rand.Next(keys.Count - 1);

        Guid randomGuid = keys[randomIndex];
        return GetCargoModelRef(randomGuid);
    }
    public void AddCargoModel(CargoModel cargoModel) => CargoCatalog.Add(cargoModel);
    public void RemoveCargoModel(Guid cargoModel) => CargoCatalog.Remove(cargoModel);
    public CargoModel GetCargoModelRef(Guid cargoModel) => CargoCatalog.GetRef(cargoModel);
    private CargoModel GetCargoModelObject(Guid cargoModel) => CargoCatalog.GetObject(cargoModel);
    private HashSet<Guid> GetAllCargoModelGuids() => CargoCatalog.GetAllGuids();

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

        HashSet<Guid> guids = GetAllStationGuids();

        Dictionary<string, Guid> stationGuids = new();
        foreach (var guid in guids)
        {
            Station station = GetStationRef(guid);
            stationGuids.Add(station.Name, station.Guid);
        }

        AddStationToStation(stationGuids["Station1"], stationGuids["Station2"], StationOrientation.Head_Tail);
        AddStationToStation(stationGuids["Station2"], stationGuids["Station3"], StationOrientation.Head_Tail);
        AddStationToStation(stationGuids["Station3"], stationGuids["Station4"], StationOrientation.Head_Head);
        AddStationToStation(stationGuids["Station4"], stationGuids["Station5"], StationOrientation.Tail_Head);
        AddStationToStation(stationGuids["Station5"], stationGuids["Station1"], StationOrientation.Tail_Tail);
    }
}
