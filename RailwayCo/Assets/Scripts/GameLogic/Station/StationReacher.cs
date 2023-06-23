using System;
using System.Collections.Generic;
using System.Linq;

public class StationReacher
{
    public DictHelper<HashsetHelper> ReacherDict { get; private set; }

    public StationReacher(WorkerDictHelper<Station> stationMaster)
    {
        ReacherDict = new();
        Bfs(stationMaster);
    }

    public void UnlinkStations(Guid station1, Guid station2)
    {
        ReacherDict.GetObject(station1).Remove(station2);
        ReacherDict.GetObject(station2).Remove(station1);
    }

    public void RemoveStation(Guid station)
    {
        List<Guid> affectedStations = ReacherDict.GetObject(station).GetAll().ToList();
        affectedStations.ForEach(affectedStation =>
        {
            ReacherDict.GetObject(affectedStation).Remove(station);
        });
        ReacherDict.Remove(station);
    }

    public void Bfs(WorkerDictHelper<Station> stationMaster)
    {
        List<Guid> stations = stationMaster.GetAllGuids().ToList();
        if (stations.Count == 0) return;

        DictHelper<bool> visitedMain = InitVisited(stations);
        stations.ForEach(station => ReacherDict.Add(station, new()));

        Guid startStation = CheckVisited(visitedMain);
        while (true)
        {
            DictHelper<bool> visited = InitVisited(stations);
            visited.Collection[startStation] = true;
            visited = BfsHelper(stationMaster, visited, startStation);

            UpdateMainStructs(visitedMain, visited);
            startStation = CheckVisited(visitedMain);
            if (startStation == Guid.Empty) break;
            visitedMain.Collection[startStation] = true;
        }
    }

    private DictHelper<bool> BfsHelper(
        WorkerDictHelper<Station> stationMaster,
        DictHelper<bool> visited,
        Guid startStation)
    {
        Queue<Guid> traversalQueue = new();
        traversalQueue.Enqueue(startStation);

        while (traversalQueue.Count != 0)
        {
            Guid targetStation = traversalQueue.Dequeue();
            visited.Collection[targetStation] = true;
            
            Station targetStationRef = stationMaster.GetRef(targetStation);
            HashSet<Guid> subStations = targetStationRef.StationHelper.GetAllGuids();
            foreach (Guid subStation in subStations)
            {
                if (!visited.GetObject(subStation)) traversalQueue.Enqueue(subStation);
            }
        }

        return visited;
    }

    private void UpdateMainStructs(
        DictHelper<bool> visitedMain,
        DictHelper<bool> visited)
    {
        List<Guid> guids = new(visited.Collection.Where(kvp => kvp.Value)
                                                        .Select(kvp => kvp.Key));
        guids.ForEach(guid =>
        {
            visitedMain.Collection[guid] = true;

            List<Guid> toSetGuids = new(guids);
            toSetGuids.Remove(guid);
            toSetGuids.ForEach(toSetGuid => ReacherDict.Collection[guid].Add(toSetGuid));
        });
    }

    private DictHelper<bool> InitVisited(List<Guid> stations)
    {
        DictHelper<bool> visited = new();
        stations.ForEach(station => visited.Add(station, false));
        return visited;
    }

    private Guid CheckVisited(DictHelper<bool> visited)
    {
        HashSet<Guid> stations = visited.GetAllGuids();
        foreach(Guid station in stations)
        {
            if (!visited.GetObject(station)) return station;
        }
        return Guid.Empty;
    }
}