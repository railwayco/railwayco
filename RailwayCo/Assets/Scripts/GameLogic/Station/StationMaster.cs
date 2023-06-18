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
            RemoveTrack(guid, _guid);
        }
        Remove(guid);
    }
    public HashSet<Guid> GetAllStation() => new(Collection.Keys);

    public void LockStation(Guid guid) => Get(guid).Lock();
    public void UnlockStation(Guid guid) => Get(guid).Unlock();
    public void OpenStation(Guid guid) => Get(guid).Open();
    public void CloseStation(Guid guid) => Get(guid).Close();

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

    public void AddTrain(Guid station, Guid train) => Get(station).TrainHelper.Add(train);
    public void RemoveTrain(Guid station, Guid train) => Get(station).TrainHelper.Remove(train);

    public void AddCargo(Guid station, Guid cargo) => Get(station).CargoHelper.Add(cargo);
    public void RemoveCargo(Guid station, Guid cargo) => Get(station).CargoHelper.Remove(cargo);
}
