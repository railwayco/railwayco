using System.Collections;
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
        List<string> tracksToRemove = stationToRemove.StationManager.StationList;
        foreach(string stnName in tracksToRemove)
        {
            StationDict[stnName].StationManager.RemoveStation(stationName);
        }
        StationDict.Remove(stationName);
    }

    public void AddTrack(Station station1, Station station2)
    {
        StationDict[station1.StationName].StationManager.AddStation(station2);
        StationDict[station2.StationName].StationManager.AddStation(station1);
    }

    public void RemoveTrack(Station station1, Station station2)
    {
        StationDict[station1.StationName].StationManager.RemoveStation(station2.StationName);
        StationDict[station2.StationName].StationManager.RemoveStation(station1.StationName);
    }
}
