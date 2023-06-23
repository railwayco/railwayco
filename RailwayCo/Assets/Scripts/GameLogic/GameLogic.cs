using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.Fields)]
public class GameLogic
{
    public event EventHandler<GameDataType> UpdateHandler;

    private User user;
    private WorkerDictHelper<Cargo> cargoMaster;
    private WorkerDictHelper<Train> trainMaster;
    private WorkerDictHelper<Station> stationMaster;
    private WorkerDictHelper<CargoModel> cargoCatalog;
    private WorkerDictHelper<TrainModel> trainCatalog;
    private StationReacher stationReacher;

    private User User { get => user; set => user = value; }
    private WorkerDictHelper<Cargo> CargoMaster { get => cargoMaster; set => cargoMaster = value; }
    private WorkerDictHelper<Train> TrainMaster { get => trainMaster; set => trainMaster = value; }
    private WorkerDictHelper<Station> StationMaster { get => stationMaster; set => stationMaster = value; }
    private WorkerDictHelper<CargoModel> CargoCatalog { get => cargoCatalog; set => cargoCatalog = value; }
    private WorkerDictHelper<TrainModel> TrainCatalog { get => trainCatalog; set => trainCatalog = value; }
    private StationReacher StationReacher { get => stationReacher; set => stationReacher = value; }

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
    private void SetCargoAssociation(Guid cargo, CargoAssociation cargoAssoc) => CargoMaster.GetObject(cargo).SetCargoAssoc(cargoAssoc);

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
    }
    public void OnTrainDeparture(Guid train, Guid sourceStation, Guid destinationStation)
    {
        // TODO: Check if train has sufficient fuel and durability

        SetTrainTravelPlan(train, sourceStation, destinationStation);
        RemoveTrainFromStation(sourceStation, train);
    }
    public Guid GetTrainDestination(Guid train) => GetTrainObject(train).TravelPlan.DestinationStation;
    public void SetTrainTravelPlan(Guid train, Guid sourceStation, Guid destinationStation)
    {
        GetTrainObject(train).TravelPlan.SetSourceStation(sourceStation);
        GetTrainObject(train).TravelPlan.SetDestinationStation(destinationStation);
    }
    public HashSet<Guid> GetAllCargoGuidsFromTrain(Guid train) => GetTrainObject(train).CargoHelper.GetAll();
    public HashSet<Guid> GetAllTrainGuids() => TrainMaster.GetAllGuids();
    public Train GetTrainRef(Guid train) => TrainMaster.GetRef(train);
    private Train GetTrainObject(Guid train) => TrainMaster.GetObject(train);
    private void AddTrain(Train train) => TrainMaster.Add(train);
    private void RemoveTrain(Guid train) => TrainMaster.Remove(train);
    private void AddCargoToTrain(Guid train, Guid cargo)
    {
        GetTrainObject(train).CargoHelper.Add(cargo);
        SetCargoAssociation(cargo, CargoAssociation.TRAIN);
        // TODO: Check train capacity
    }
    private void RemoveCargoFromTrain(Guid train, Guid cargo)
    {
        GetTrainObject(train).CargoHelper.Remove(cargo);
        SetCargoAssociation(cargo, CargoAssociation.NIL);
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

    /// <summary> This method adds a track between 2 stations such that station2 is at the 
    /// head of station1, where the head is denoted as the right side of the station when 
    /// placed horizontally </summary>
    public void AddStationToStation(Guid station1, Guid station2)
    {
        GetStationObject(station1).StationHelper.Add(station2, StationOrientation.Head);
        GetStationObject(station2).StationHelper.Add(station1, StationOrientation.Tail);
        StationReacher = new(StationMaster); // TODO: optimise this in the future
    }
    public void RemoveStationFromStation(Guid station1, Guid station2)
    {
        GetStationObject(station1).StationHelper.Remove(station2);
        GetStationObject(station2).StationHelper.Remove(station1);
        StationReacher.UnlinkStations(station1, station2);
    }
    public HashSet<Guid> GetAllStationGuidsFromStation(Guid station)
    {
        return new(GetStationObject(station).StationHelper.GetAllGuids());
    }
    public void AddTrainToStation(Guid station, Guid train) => GetStationObject(station).TrainHelper.Add(train);
    public void RemoveTrainFromStation(Guid station, Guid train) => GetStationObject(station).TrainHelper.Remove(train);
    public HashSet<Guid> GetAllCargoGuidsFromStation(Guid station) => GetStationObject(station).CargoHelper.GetAll();
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
    }
    public HashSet<Guid> GetAllStationGuids() => StationMaster.GetAllGuids();
    public Station GetStationRef(Guid station) => StationMaster.GetRef(station);
    private Station GetStationObject(Guid station) => StationMaster.GetObject(station);
    private void AddStation(Station station)
    {
        StationMaster.Add(station);
        if (station.StationHelper.Collection.Count > 0) StationReacher.Bfs(StationMaster);
        // TODO: Check if all stations in StationHelper exists before running Bfs
    }
    private void RemoveStation(Guid station)
    {
        StationMaster.Remove(station);
        StationReacher.RemoveStation(station);
    }
    private void AddCargoToStation(Guid station, Guid cargo)
    {
        Station stationObject = GetStationObject(station);
        stationObject.CargoHelper.Add(cargo);

        Cargo cargoRef = GetCargoRef(cargo);
        if (!cargoRef.TravelPlan.IsAtSource(station))
        {
            stationObject.AddToYard();
            SetCargoAssociation(cargo, CargoAssociation.YARD);
        }
        else
        {
            SetCargoAssociation(cargo, CargoAssociation.STATION);
        }
    }
    private void RemoveCargoFromStation(Guid station, Guid cargo)
    {
        Station stationObject = GetStationObject(station);
        stationObject.CargoHelper.Remove(cargo);

        SetCargoAssociation(cargo, CargoAssociation.NIL);

        Cargo cargoRef = GetCargoRef(cargo);
        if (!cargoRef.TravelPlan.IsAtSource(station)) stationObject.RemoveFromYard();
    }

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

    private void SendDataForUpdate(GameDataType gameDataType)
    {
        switch(gameDataType)
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


    /////////// QUICK FIX ///////////
    public Guid saveStationInfo(string stationName)
    {
        Station station = new(
                stationName,
                StationStatus.Open,
                new(),
                new(),
                new(),
                new(0, 5, 0, 0));
        StationMaster.Add(station);
        return station.Guid;
    }

    public Guid saveTrainInfo(string trainName)
    {
        TrainAttribute attribute = new(
            new(0, 4, 0, 0),
            new(0.0, 100.0, 100.0, 5.0),
            new(0.0, 100.0, 100.0, 5.0),
            new(0.0, 200.0, 0.0, 0.0));
        Train train = new(
            trainName,
            TrainType.Steam,
            attribute,
            new());
        TrainMaster.Add(train);
        return train.Guid;
    }

    public void GenerateRandomData()
    {
        Random rand = new();
        CargoType[] cargoTypes = (CargoType[])Enum.GetValues(typeof(CargoType));
        CurrencyType[] currencyTypes = (CurrencyType[])Enum.GetValues(typeof(CurrencyType));
        foreach (var cargoType in cargoTypes)
        {
            CurrencyManager currencyManager = new();
            CurrencyType randomType = currencyTypes[rand.Next(currencyTypes.Length)];
            double randomAmount = rand.Next(500, 5000);
            Currency currency = new(randomType, randomAmount);
            currencyManager.AddCurrency(currency);

            CargoModel cargoModel = new(cargoType, 15, 20, currencyManager);
            CargoCatalog.Add(cargoModel);
        }

        SendDataForUpdate(GameDataType.CargoCatalog);
    }
}
