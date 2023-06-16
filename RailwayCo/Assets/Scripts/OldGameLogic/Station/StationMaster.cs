using System.Collections.Generic;

public class StationMaster
{
    private Dictionary<string, Station> stationDict;

    private Dictionary<string, Station> StationDict { get => stationDict; set => stationDict = value; }
    public List<string> StationList => new(StationDict.Keys);

    public StationMaster(Dictionary<string, Station> stationDict) => StationDict = stationDict;

    public void AddStation(string stationName)
    {
        Station newStation = new(stationName, StationStatus.Locked, new(), new(), new());
        StationDict.Add(stationName, newStation);
    }

    public void RemoveStation(string stationName)
    {
        Station stationToRemove = StationDict[stationName];
        HashSet<string> tracksToRemove = stationToRemove.StationManager.StationList;
        foreach(string stnName in tracksToRemove)
        {
            StationDict[stnName].StationManager.RemoveStation(stationName);
        }
        StationDict.Remove(stationName);
    }

    public Station GetStation(string stationName) => StationDict[stationName];

    public void UpdateStation(Station station) => StationDict[station.StationName] = station;

    public void AddTrack(string stationName1, string stationName2)
    {
        StationDict[stationName1].StationManager.AddStation(stationName2);
        StationDict[stationName2].StationManager.AddStation(stationName1);
    }

    public void RemoveTrack(string stationName1, string stationName2)
    {
        StationDict[stationName1].StationManager.RemoveStation(stationName2);
        StationDict[stationName2].StationManager.RemoveStation(stationName1);
    }
}
