using System;
using System.Collections.Generic;
using System.Linq;

public class StationReacher
{
    public DictHelper<HashsetHelper> ReacherDict { get; set; }

    public StationReacher(WorkerDictHelper<Station> stationMaster)
    {
        ReacherDict = new();
        Bfs(stationMaster);
    }

    private void Bfs(WorkerDictHelper<Station> stationMaster)
    {
        DictHelper<bool> visited = new();
        List<Guid> stations = stationMaster.GetAllGuids().ToList();
        if (stations.Count == 0) return;
        
        foreach (Guid station in stations)
        {
            visited.Add(station, false);
            ReacherDict.Add(station, new());
        }
        
        while (true)
        {
            Guid startStation = CheckVisited(visited);
            if (startStation == Guid.Empty) break;
            visited.Collection[startStation] = true;
            BfsHelper(stationMaster, visited, startStation);

        }
            
    }

    private void BfsHelper(
        WorkerDictHelper<Station> stationMaster,
        DictHelper<bool> visited,
        Guid startStation)
    {
        Queue<Guid> traversalQueue = new();
        traversalQueue.Enqueue(startStation);

        while (traversalQueue.Count != 0)
        {
            Guid targetStation = traversalQueue.Dequeue();
            Station targetStationRef = stationMaster.GetRef(targetStation);
            HashSet<Guid> subStations = targetStationRef.StationHelper.GetAllGuids();

            foreach (Guid subStation in subStations)
            {
                ReacherDict.Collection[targetStation].Add(subStation);
                ReacherDict.Collection[subStation].Add(targetStation);
                visited.Collection[subStation] = true;

                Station subStationRef = stationMaster.GetRef(subStation);
                HashSet<Guid> subSubStations = subStationRef.StationHelper.GetAllGuids();
                foreach (Guid subSubstation in subSubStations)
                {
                    if (!visited.GetObject(subSubstation)) traversalQueue.Enqueue(subSubstation);
                }
            }
        }
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
