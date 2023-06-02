using System.Collections;
using System.Collections.Generic;

public class Station
{
    private string stationName;
    private StationStatus stationStatus;
    private TrainManager trainManager;
    private CargoManager cargoManager;

    public string StationName { get => stationName; private set => stationName = value; }
    public StationStatus StationStatus { get => stationStatus; private set => stationStatus = value; }
    private TrainManager TrainManager { get => trainManager; set => trainManager = value; }
    private CargoManager CargoManager { get => cargoManager; set => cargoManager = value; }

    public Station(
        string stationName,
        StationStatus stationStatus)
    {
        StationName = stationName;
        StationStatus = stationStatus;
        TrainManager = new();
        CargoManager = new();
    }

    public void TrainArrival(Train train)
    {
        TrainManager.AddTrain(train);

        List<Cargo> cargoList = train.CargoManager.GetArrivedCargo(StationName);

        // TODO: Claim rewards

        train.CargoManager.RemoveSelectedCargo(cargoList);
    }

    public void TrainDeparture(Train train)
    {
        // TODO: Check if train has sufficient fuel
            // Sum up total fuel consumption
            // Then check the sum against fuel level
        
        TrainManager.RemoveTrain(train);
    }
}

public enum StationStatus
{
    Locked,
    Open,
    Closed
}

// TODO: Station Mesh -- Maintain a graph of all Station tracks

// TODO: Station List -- List of all Station
