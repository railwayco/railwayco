using System.Collections.Generic;

public class StationManager
{
    private HashSet<string> stationList;

    public HashSet<string> StationList { get => stationList; set => stationList = value; }

    public StationManager() => StationList = new();

    public void AddStation(string stationName) => StationList.Add(stationName);

    public void RemoveStation(string stationName) => StationList.Remove(stationName);
}
