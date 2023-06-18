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

    public void AddTrain(Guid stationGuid, Guid trainGuid)
    {
        Get(stationGuid).TrainHelper.Add(trainGuid);
    }

    public void RemoveTrain(Guid stationGuid, Guid trainGuid)
    {
        Get(stationGuid).TrainHelper.Remove(trainGuid);
    }

    public void AddCargo(Guid stationGuid, Guid cargoGuid)
    {
        Get(stationGuid).CargoHelper.Add(cargoGuid);
    }

    public void RemoveCargo(Guid stationGuid, Guid cargoGuid)
    {
        Get(stationGuid).CargoHelper.Remove(cargoGuid);
    }
}
