using System;
using System.Collections.Generic;

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
            Station station = StationMaster.Init();
            station.Name = "Station" + (i + 1).ToString();
            StationMaster.AddStation(station);
        }

        int NUM_OF_TRAINS = 8;
        for (int i = 0; i < NUM_OF_TRAINS; i++)
        {
            TrainAttribute attribute = new(
            new(0, 4, 0, 0),
            new(0.0, 100.0, 100.0, 5.0),
            new(0.0, 100.0, 100.0, 5.0),
            new(0.0, 200.0, 0.0, 0.0));
            Train train = TrainMaster.Init(
                "Train" + (i + 1).ToString(), 
                TrainType.Steam, 
                attribute, 
                new());
            TrainMaster.AddTrain(train);
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

            CargoModel cargoModel = CargoCatalog.Init(cargoType, 15, 20, currencyManager);
            CargoCatalog.AddCargoModel(cargoModel);
        }
    }

    public HashSet<Guid> GetAllCargo() => CargoMaster.GetAllCargo();
    public Cargo GetCargo(Guid cargo) => CargoMaster.GetCargo(cargo);
    public HashSet<Guid> GetAllTrain() => TrainMaster.GetAllTrain();
    public Train GetTrain(Guid train) => TrainMaster.GetTrain(train);
    public HashSet<Guid> GetAllStation() => StationMaster.GetAllStation();
    public Station GetStation(Guid station) => StationMaster.GetStation(station);

    public void GenerateNewCargo(int numOfNewCargoPerStation)
    {
        HashSet<Guid> stations = StationMaster.GetAllStation();
        foreach(Guid station in stations)
        {
            List<Guid> subStations = new(stations);
            subStations.Remove(station);
            Random rand = new();

            for (int i = 0; i < numOfNewCargoPerStation; i++)
            {
                CargoModel cargoModel = CargoCatalog.GetRandomCargoModel();
                Guid destination = subStations[rand.Next(subStations.Count - 1)];
                Cargo cargo = CargoMaster.Init(cargoModel, station, destination);
                CargoMaster.AddCargo(cargo);
            }
        }
    }

    public void MoveCargoFromStationtoTrain(Guid cargo, Guid station, Guid train)
    {
        StationMaster.RemoveCargo(station, cargo);
        TrainMaster.AddCargo(train, cargo);
    }

    public void MoveCargoFromTrainToStation(Guid cargo, Guid train, Guid station)
    {
        TrainMaster.RemoveCargo(train, cargo);
        StationMaster.AddCargo(station, cargo);
    }

    public void OnTrainArrival(Guid train)
    {
        Guid station = TrainMaster.GetDestination(train);
        StationMaster.AddTrain(station, train);

        HashSet<Guid> cargoCollection = TrainMaster.GetAllCargo(train);
        cargoCollection = CargoMaster.FilterCargoHasArrived(cargoCollection, station);

        CurrencyManager total = CargoMaster.GetCurrencyManagerForCargoRange(cargoCollection);
        User.CurrencyManager.AddCurrencyManager(total);

        TrainMaster.RemoveCargoRange(train, cargoCollection);
        CargoMaster.RemoveCargoRange(cargoCollection);
    }

    public void OnTrainDeparture(Guid train, Guid sourceStation, Guid destinationStation)
    {
        // TODO: Check if train has sufficient fuel and durability

        TrainMaster.SetTravelPlan(train, sourceStation, destinationStation);
        StationMaster.RemoveTrain(sourceStation, train);
    }
}
