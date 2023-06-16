using System;
using System.Collections.Generic;

public class StationMaster : Master<Station>
{
    public StationMaster() => Collection = new();

    public Station Init() => new("", StationStatus.Locked, new(), new(), new());

    public void AddStation(Station station) => Add(station.Guid, station);

    public void RemoveStation(Guid guid)
    {
        Station stationToRemove = Get(guid);
        HashSet<Guid> tracksToRemove = stationToRemove.StationHelper.GetAll();
        foreach(Guid _guid in tracksToRemove)
        {
            Station relatedStation = Get(_guid);
            relatedStation.StationHelper.Remove(guid);
        }
        Remove(guid);
    }

    public void AddTrack(Guid guid1, Guid guid2)
    {
        Station station1 = Get(guid1);
        Station station2 = Get(guid2);
        station1.StationHelper.Add(guid2);
        station2.StationHelper.Add(guid1);
    }

    public void RemoveTrack(Guid guid1, Guid guid2)
    {
        Station station1 = Get(guid1);
        Station station2 = Get(guid2);
        station1.StationHelper.Remove(guid2);
        station2.StationHelper.Remove(guid1);
    }
}
