using System.Collections.Generic;

public class Station
{
    private string stationName;
    private StationStatus stationStatus;
    private StationManager stationManager;
    private TrainManager trainManager;
    private CargoManager cargoManager;

    public string StationName { get => stationName; private set => stationName = value; }
    public StationStatus StationStatus { get => stationStatus; private set => stationStatus = value; }
    public StationManager StationManager { get => stationManager; set => stationManager = value; }
    private TrainManager TrainManager { get => trainManager; set => trainManager = value; }
    private CargoManager CargoManager { get => cargoManager; set => cargoManager = value; }

    public Station(
        string stationName,
        StationStatus stationStatus,
        StationManager stationManager,
        TrainManager trainManager,
        CargoManager cargoManager)
    {
        StationName = stationName;
        StationStatus = stationStatus;
        StationManager = stationManager;
        TrainManager = trainManager;
        CargoManager = cargoManager;
    }

    public void StationOpened() => StationStatus = StationStatus.Open;
    public void StationClosed() => StationStatus = StationStatus.Closed;
    public void StationLocked() => StationStatus = StationStatus.Locked;
    public void StationUnlocked() => StationOpened();

    public List<Cargo> ReloadCargoList()
    {
        // TODO: Populate station with new cargo

        return CargoManager.CargoList;
    }

    public void TrainLoadCargo(Train train, Cargo cargo)
    {
        CargoManager.RemoveSelectedCargo(new List<Cargo> { cargo });
        train.CargoManager.AddCargo(cargo);
    }

    public void TrainUnloadCargo(Train train, Cargo cargo)
    {
        train.CargoManager.RemoveSelectedCargo(new List<Cargo> { cargo });
        CargoManager.AddCargo(cargo);
    }

    public CurrencyManager TrainArrival(Train train)
    {
        TrainManager.AddTrain(train);
        List<Cargo> cargoList = train.CargoManager.GetArrivedCargo(StationName);
        train.CargoManager.RemoveSelectedCargo(cargoList);

        CurrencyManager currencyManager = new();
        foreach (Cargo c in cargoList)
        {
            currencyManager.AddCurrencyManager(c.CurrencyManager);
        }
        return currencyManager;
    }

    public void TrainDeparture(Train train)
    {
        // TODO: Check if train has sufficient fuel
        // Sum up total fuel consumption
        // Then check the sum against fuel level

        TrainManager.RemoveTrain(train);
    }
}
