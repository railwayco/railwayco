using System;
using System.Collections.Generic;
using System.Linq;

public class StationMaster
{
    private WorkerDictHelper<Station> Collection { get; set; }

    public StationMaster()
    {
        Collection = new();
    }

    public Station GetObject(int stationNum)
    {
        Station station = default;
        HashSet<Guid> stations = Collection.GetAll();
        foreach (var guid in stations)
        {
            Station stationObject = Collection.GetRef(guid);
            if (stationObject.Number.Equals(stationNum))
            {
                station = stationObject;
                break;
            }
        }
        return station;
    }
    
    public Station GetObject(Guid station) => Collection.GetRef(station);

    public Guid AddObject(int stationNumber, PlatformMaster platformMaster)
    {
        StationAttribute stationAttribute = new(
            new(0, 5, 0, 0));
        Station station = new(
                stationNumber,
                OperationalStatus.Open,
                stationAttribute,
                new(),
                new(),
                new());

        Collection.Add(station);
        StationReacher.Bfs(Collection, platformMaster);
        return station.Guid;
    }

    #region Status Management
    public OperationalStatus GetStationStatus(Guid station) => Collection.GetObject(station).Status;
    public void CloseStation(Guid station) => Collection.GetObject(station).Close();
    public void OpenStation(Guid station) => Collection.GetObject(station).Open();
    public void LockStation(Guid station)
    {
        Collection.GetObject(station).Lock();
        StationReacher.DisconnectStation(Collection, station);
    }
    public void UnlockStation(Guid station, PlatformMaster platformMaster)
    {
        Collection.GetObject(station).Unlock();
        StationReacher.Bfs(Collection, platformMaster);
    }
    #endregion

    #region Cargo Management
    public IEnumerator<Guid> GetRandomDestinations(Guid station, int numOfDestinations)
    {
        List<Guid> reachableStations = Collection.GetObject(station).StationHelper.GetAll().ToList();
        if (reachableStations.Count == 0)
            yield break;

        Random rand = new();
        for (int i = 0; i < numOfDestinations; i++)
        {
            int numOfReachableStations = reachableStations.Count;
            yield return reachableStations[rand.Next(numOfReachableStations)];
        }
    }
    public void AddCargoToStation(Guid station, Guid cargo)
    {
        Station stationObject = Collection.GetObject(station);
        stationObject.CargoHelper.Add(cargo);
    }
    public void RemoveCargoFromStation(Guid station, Guid cargo)
    {
        Station stationObject = Collection.GetObject(station);
        stationObject.CargoHelper.Remove(cargo);
    }
    public bool AddCargoToYard(Guid station, Guid cargo)
    {
        Station stationObject = Collection.GetObject(station);
        if (stationObject.Attribute.IsYardFull())
            return false;
        stationObject.Attribute.AddToYard();
        stationObject.CargoHelper.Add(cargo);
        return true;
    }
    public void RemoveCargoFromYard(Guid station, Guid cargo)
    {
        Station stationObject = Collection.GetObject(station);
        stationObject.Attribute.RemoveFromYard();
        stationObject.CargoHelper.Remove(cargo);
    }
    #endregion

    #region Train Management
    public void AddTrainToStation(Guid station, Guid train)
    {
        Station stationObject = Collection.GetObject(station);
        stationObject.TrainHelper.Add(train);
    }
    public void RemoveTrainFromStation(Guid station, Guid train)
    {
        Station stationObject = Collection.GetObject(station);
        stationObject.TrainHelper.Remove(train);
    }
    #endregion
}
