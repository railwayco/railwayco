using System;
using System.Collections.Generic;
using System.Linq;

public class GameLogic
{
    private User User { get; set; }
    private DictHelper<Cargo> CargoMaster { get; set; }
    private DictHelper<Train> TrainMaster { get; set; }
    private DictHelper<Station> StationMaster { get; set; }
    private DictHelper<CargoModel> CargoCatalog { get; set; }
    private DictHelper<TrainModel> TrainCatalog { get; set; }

    public GameLogic()
    {
        // Temporary solution to get dummy data

        User = new("", 0, 0, new());
        CargoMaster = new();
        TrainMaster = new();
        StationMaster = new();
        CargoCatalog = new();
        TrainCatalog = new();

        int NUM_OF_STATIONS = 8;
        for (int i = 0; i < NUM_OF_STATIONS; i++)
        {
            Station station = new(
                "Station" + (i + 1).ToString(), 
                StationStatus.Open,
                new(),
                new(),
                new());
            StationMaster.Add(station);
        }

        int NUM_OF_TRAINS = 8;
        for (int i = 0; i < NUM_OF_TRAINS; i++)
        {
            TrainAttribute attribute = new(
            new(0, 4, 0, 0),
            new(0.0, 100.0, 100.0, 5.0),
            new(0.0, 100.0, 100.0, 5.0),
            new(0.0, 200.0, 0.0, 0.0));
            Train train = new(
                "Train" + (i + 1).ToString(), 
                TrainType.Steam, 
                attribute, 
                new());
            TrainMaster.Add(train);
        }

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
    }

    public HashSet<Guid> GetAllCargoGuids() => CargoMaster.GetAllGuids();
    public Cargo GetCargoRef(Guid cargo) => CargoMaster.GetRef(cargo);
    private Cargo GetCargoObject(Guid cargo) => CargoMaster.GetObject(cargo);
    private void AddCargo(Cargo cargo) => CargoMaster.Add(cargo);
    private void RemoveCargo(Guid cargo) => CargoMaster.Remove(cargo);

    public void OnTrainArrival(Guid train)
    {
        Guid station = GetTrainDestination(train);
        CurrencyManager totalCurrency = new();

        AddTrainToStation(station, train);
        HashSet<Guid> cargoCollection = GetAllCargoGuidsFromTrain(train);
        foreach (Guid cargo in cargoCollection)
        {
            Cargo cargoRef = GetCargoRef(cargo);
            if (!cargoRef.TravelPlan.HasArrived(station)) continue;

            totalCurrency.AddCurrencyManager(cargoRef.CurrencyManager);
            RemoveCargoFromTrain(train, cargo);
            RemoveCargo(cargo);
        }
        User.CurrencyManager.AddCurrencyManager(totalCurrency);
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

    public void AddTrackToStation(Guid station1, Guid station2)
    {
        GetStationObject(station1).StationHelper.Add(station2);
        GetStationObject(station2).StationHelper.Add(station1);
    }
    public void RemoveTrackFromStation(Guid station1, Guid station2)
    {
        GetStationObject(station1).StationHelper.Remove(station2);
        GetStationObject(station1).StationHelper.Remove(station2);
    }
    public void AddTrainToStation(Guid station, Guid train) => GetStationObject(station).TrainHelper.Add(train);
    public void RemoveTrainFromStation(Guid station, Guid train) => GetStationObject(station).TrainHelper.Remove(train);
    public void AddCargoToStation(Guid station, Guid cargo) => GetStationObject(station).CargoHelper.Add(cargo);
    public void RemoveCargoFromStation(Guid station, Guid cargo) => GetStationObject(station).CargoHelper.Remove(cargo);
    public void AddRandomCargoToStation(Guid station, int numOfRandomCargo)
    {
        Station stationObject = GetStationObject(station);
        List<Guid> subStations = GetAllStationGuids().ToList();
        subStations.Remove(station);
        Random rand = new();

        for (int i = 0; i < numOfRandomCargo; i++)
        {
            CargoModel cargoModel = GetRandomCargoModel();
            cargoModel.Randomise();
            Guid destination = subStations[rand.Next(subStations.Count - 1)];

            Cargo cargo = new(cargoModel, station, destination);
            CargoMaster.Add(cargo);
            stationObject.StationHelper.Add(cargo.Guid);
        }
    }
    public HashSet<Guid> GetAllStationGuids() => StationMaster.GetAllGuids();
    public Station GetStationRef(Guid station) => StationMaster.GetRef(station);
    private Station GetStationObject(Guid station) => StationMaster.GetObject(station);

    public CargoModel GetRandomCargoModel()
    {
        List<Guid> keys = GetAllCargoModelGuids().ToList();

        Random rand = new();
        int randomIndex = rand.Next(keys.Count - 1);

        Guid randomGuid = keys[randomIndex];
        return GetCargoModelRef(randomGuid);
    }
    public CargoModel GetCargoModelRef(Guid cargoModel) => CargoCatalog.GetRef(cargoModel);
    private CargoModel GetCargoModelObject(Guid cargoModel) => CargoCatalog.GetObject(cargoModel);
    private HashSet<Guid> GetAllCargoModelGuids() => CargoCatalog.GetAllGuids();
}
