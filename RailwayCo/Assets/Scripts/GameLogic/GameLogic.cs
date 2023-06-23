using System;
using System.Collections.Generic;
using System.Linq;

public class GameLogic
{
    private User User { get; set; }
    private WorkerDictHelper<Cargo> CargoMaster { get; set; }
    private WorkerDictHelper<Train> TrainMaster { get; set; }
    private WorkerDictHelper<Station> StationMaster { get; set; }
    private WorkerDictHelper<CargoModel> CargoCatalog { get; set; }
    private WorkerDictHelper<TrainModel> TrainCatalog { get; set; }
    private StationReacher StationReacher { get; set; }

    public GameLogic()
    {
        User = new("", 0, 0, new());
        CargoMaster = new();
        TrainMaster = new();
        StationMaster = new();
        CargoCatalog = new();
        TrainCatalog = new();
        StationReacher = new(StationMaster);

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
            AddCargoModel(cargoModel);
        }
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
    public void AddCargoToTrain(Guid train, Guid cargo) => GetTrainObject(train).CargoHelper.Add(cargo);
    public void RemoveCargoFromTrain(Guid train, Guid cargo) => GetTrainObject(train).CargoHelper.Remove(cargo);
    public HashSet<Guid> GetAllCargoGuidsFromTrain(Guid train) => GetTrainObject(train).CargoHelper.GetAll();
    public HashSet<Guid> GetAllTrainGuids() => TrainMaster.GetAllGuids();
    public Train GetTrainRef(Guid train) => TrainMaster.GetRef(train);
    private Train GetTrainObject(Guid train) => TrainMaster.GetObject(train);
    private void AddTrain(Train train) => TrainMaster.Add(train);
    private void RemoveTrain(Guid train) => TrainMaster.Remove(train);

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
    public void AddCargoToStation(Guid station, Guid cargo) => GetStationObject(station).CargoHelper.Add(cargo);
    public void RemoveCargoFromStation(Guid station, Guid cargo) => GetStationObject(station).CargoHelper.Remove(cargo);
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

            Cargo cargo = new(cargoModel, station, destination, Cargo.CargoAssociation.CARGO_STATION);
            CargoMaster.Add(cargo);
            stationObject.CargoHelper.Add(cargo.Guid);
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


    /////////// QUICK FIX ///////////
    public Guid saveStationInfo(string stationName)
    {
        Station station = new(
                stationName,
                StationStatus.Open,
                new(),
                new(),
                new());
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



}
