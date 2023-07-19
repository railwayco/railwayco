using System;
using System.Collections.Generic;
using System.Linq;

public class StationMaster : IPlayfab
{
    private WorkerDictionary<Station> Collection { get; set; }

    public StationMaster() => Collection = new();

    #region Collection Management
    public HashSet<Guid> GetAllGuids() => new(Collection.Keys);
    public Station GetObject(int stationNum)
    {
        Station station = default;
        HashSet<Guid> stations = GetAllGuids();
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
    public Guid AddObject(int stationNumber)
    {
        StationAttribute stationAttribute = new(
            new(0, 5, 0, 0));
        Station station = new(
                stationNumber,
                stationAttribute,
                new(),
                new(),
                new(),
                new());

        Collection.Add(station);
        return station.Guid;
    }
    #endregion

    #region Cargo Management
    public IEnumerator<Guid> GetRandomDestinations(Guid station, int numDestinations)
    {
        List<Guid> reachableStations = Collection.GetObject(station).StationHelper.ToList();
        int numReachableStations = reachableStations.Count;
        if (numReachableStations == 0)
            yield break;

        Random rand = new();
        for (int i = 0; i < numDestinations; i++)
        {
            yield return reachableStations[rand.Next(numReachableStations)];
        }
    }
    public void AddCargoToStation(Guid station, Guid cargo)
    {
        Station stationObject = Collection.GetObject(station);
        stationObject.StationCargoHelper.Add(cargo);
    }
    public void RemoveCargoFromStation(Guid station, Guid cargo)
    {
        Station stationObject = Collection.GetObject(station);
        stationObject.StationCargoHelper.Remove(cargo);
    }
    public bool AddCargoToYard(Guid station, Guid cargo)
    {
        Station stationObject = Collection.GetObject(station);
        if (stationObject.Attribute.IsYardFull())
            return false;
        stationObject.Attribute.AddToYard();
        stationObject.YardCargoHelper.Add(cargo);
        return true;
    }
    public void RemoveCargoFromYard(Guid station, Guid cargo)
    {
        Station stationObject = Collection.GetObject(station);
        stationObject.Attribute.RemoveFromYard();
        stationObject.YardCargoHelper.Remove(cargo);
    }
    public HashSet<Guid> GetStationCargoManifest(Guid station)
    {
        Station stationObject = Collection.GetObject(station);
        return new(stationObject.StationCargoHelper);
    }
    public HashSet<Guid> GetYardCargoManifest(Guid station)
    {
        Station stationObject = Collection.GetObject(station);
        return new(stationObject.YardCargoHelper);
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

    #region Station Management
    public void AddStationToStation(Guid station1, Guid station2)
    {
        Station stationObject = Collection.GetObject(station1);
        stationObject.StationHelper.Add(station2);
    }
    public void RemoveStationFromStation(Guid station1, Guid station2)
    {
        Station stationObject = Collection.GetObject(station1);
        stationObject.StationHelper.Remove(station2);
    }
    #endregion

    #region PlayFab Management
    public string SendDataToPlayfab() => GameDataManager.Serialize(Collection);
    public void SetDataFromPlayfab(string data)
    {
        Collection = GameDataManager.Deserialize<WorkerDictionary<Station>>(data);
    }
    #endregion
}
