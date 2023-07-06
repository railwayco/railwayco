using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class StationReacher
{
    public DictHelper<HashsetHelper> ReacherDict { get; private set; }

    [JsonConstructor]
    private StationReacher(DictHelper<HashsetHelper> reacherDict) => ReacherDict = reacherDict;

    public StationReacher(WorkerDictHelper<Station> stationMaster)
    {
        ReacherDict = new();
        Bfs(stationMaster);
    }

    public void UnlinkStations(Guid station1, Guid station2)
    {
        ReacherDict.RWLock.AcquireWriterLock();
        ReacherDict.GetObject(station1).Remove(station2);
        ReacherDict.GetObject(station2).Remove(station1);
        ReacherDict.RWLock.ReleaseWriterLock();
    }

    public void RemoveStation(Guid station)
    {
        ReacherDict.RWLock.AcquireWriterLock();

        List<Guid> affectedStations = ReacherDict.GetObject(station).GetAll().ToList();
        affectedStations.ForEach(affectedStation =>
        {
            ReacherDict.GetObject(affectedStation).Remove(station);
        });
        ReacherDict.Remove(station);

        ReacherDict.RWLock.ReleaseWriterLock();
    }

    public void Bfs(WorkerDictHelper<Station> stationMaster)
    {
        ReacherDict.RWLock.AcquireWriterLock();

        List<Guid> stations = stationMaster.GetAll().ToList();
        if (stations.Count == 0)
        {
            ReacherDict.RWLock.ReleaseWriterLock();
            return;
        }

        DictHelper<bool> visitedMain = InitVisited(stations);
        stations.ForEach(station => ReacherDict.Add(station, new()));

        Guid startStation = CheckVisited(visitedMain);
        while (true)
        {
            DictHelper<bool> visited = InitVisited(stations);
            visited.Update(startStation, true);
            visited = BfsHelper(stationMaster, visited, startStation);

            UpdateMainStructs(visitedMain, visited);
            startStation = CheckVisited(visitedMain);
            if (startStation == Guid.Empty) break;
            visitedMain.Update(startStation, true);
        }

        ReacherDict.RWLock.ReleaseWriterLock();
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
            visited.Update(targetStation, true);

            Station targetStationRef = stationMaster.GetRef(targetStation);
            HashSet<Guid> subStations = targetStationRef.StationHelper.GetAll();
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
        List<Guid> guids = visited.GetAll().ToList();
        guids = new(guids.Where(guid => visited.GetObject(guid)));

        guids.ForEach(guid =>
        {
            visitedMain.Update(guid, true);

            List<Guid> toSetGuids = new(guids);
            toSetGuids.Remove(guid);
            toSetGuids.ForEach(toSetGuid => ReacherDict.GetObject(guid).Add(toSetGuid));
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
        HashSet<Guid> stations = visited.GetAll();
        foreach (Guid station in stations)
        {
            if (!visited.GetObject(station)) return station;
        }
        return Guid.Empty;
    }
}
