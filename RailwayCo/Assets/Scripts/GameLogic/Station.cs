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
}

public enum StationStatus
{
    Locked,
    Open,
    Closed
}
