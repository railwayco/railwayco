using System.Collections.Generic;

public class StationManager
{
    private List<string> stationList;

    public List<string> StationList { get => stationList; set => stationList = value; }

    public StationManager() => StationList = new();

    public void AddStation(string stationName) => StationList.Add(stationName);

    public void RemoveStation(string stationName) => StationList.Remove(stationName);
}
