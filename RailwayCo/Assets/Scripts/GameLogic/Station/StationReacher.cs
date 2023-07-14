using System;
using System.Collections.Generic;
using System.Linq;

public static class StationReacher
{
    public static void UnlinkStations(WorkerDictHelper<Station> stationMaster, Guid station1, Guid station2)
    {
        Station station1Object = stationMaster.GetObject(station1);
        Station station2Object = stationMaster.GetObject(station2);
        station1Object.StationHelper.Remove(station2);
        station2Object.StationHelper.Remove(station1);
    }

    public static void DisconnectStation(WorkerDictHelper<Station> stationMaster, Guid station)
    {
        List<Guid> affectedStations = stationMaster.GetObject(station).StationHelper.GetAll().ToList();
        affectedStations.ForEach(affectedStation =>
        {
            stationMaster.GetObject(affectedStation).StationHelper.Remove(station);
        });
    }

    public static void Bfs(WorkerDictHelper<Station> stationMaster, PlatformMaster platformMaster)
    {
        List<Guid> stations = stationMaster.GetAll().ToList();
        if (stations.Count == 0)
            return;

        List<Tuple<Guid, int>> stationGuidsAndNums = new();
        stations.ForEach(station =>
        {
            int stationNum = stationMaster.GetRef(station).Number;
            stationGuidsAndNums.Add(new(station, stationNum));
        });

        Dictionary<Tuple<Guid, int>, bool> visitedMain = InitVisited(stationGuidsAndNums);

        Tuple<Guid, int> startStation = CheckVisited(visitedMain);
        do
        {
            Dictionary<Tuple<Guid, int>, bool> visited = InitVisited(stationGuidsAndNums);
            visited[startStation] = true;
            visited = BfsHelper(stationMaster, platformMaster, visited, startStation);

            visitedMain = UpdateMainStructs(stationMaster, visitedMain, visited);
            startStation = CheckVisited(visitedMain);
        } while (startStation != default);
    }

    private static Dictionary<Tuple<Guid, int>, bool> BfsHelper(
        WorkerDictHelper<Station> stationMaster,
        PlatformMaster platformMaster,
        Dictionary<Tuple<Guid, int>, bool> visited,
        Tuple<Guid, int> startStation)
    {
        Queue<Tuple<Guid, int>> traversalQueue = new();
        traversalQueue.Enqueue(startStation);

        while (traversalQueue.Count != 0)
        {
            Tuple<Guid, int> targetStation = traversalQueue.Dequeue();
            int targetStationNum = targetStation.Item2;
            visited[targetStation] = true;

            List<Guid> platforms = platformMaster.GetPlatformsByStationNum(targetStationNum).ToList();
            platforms.ForEach(platform =>
            {
                HashSet<Track> tracks = platformMaster.GetPlatformTracks(platform);
                foreach (var track in tracks)
                {
                    if (track.Status == OperationalStatus.Locked) continue;
                    int neighbourNum = platformMaster.GetPlatformStationNum(track.Platform);
                    try
                    {
                        Tuple<Guid, int> neighbourTuple = GetTuple(visited, neighbourNum);
                        if (!visited[neighbourTuple])
                            traversalQueue.Enqueue(neighbourTuple);
                    }
                    catch (InvalidProgramException)
                    {
                        // Program is still initialising the rest of the stations
                        continue;
                    }
                }
            });
        }
        return visited;
    }

    private static Dictionary<Tuple<Guid, int>, bool> UpdateMainStructs(
        WorkerDictHelper<Station> stationMaster,
        Dictionary<Tuple<Guid, int>, bool> visitedMain,
        Dictionary<Tuple<Guid, int>, bool> visited)
    {
        List<Tuple<Guid, int>> pairs = visited.Keys.ToList();
        pairs = new(pairs.Where(pair => visited[pair]));

        pairs.ForEach(pair =>
        {
            visitedMain[pair] = true;

            List<Tuple<Guid, int>> toSetGuids = new(pairs);
            toSetGuids.Remove(pair);

            HashsetHelper stationHelper = stationMaster.GetObject(pair.Item1).StationHelper;
            toSetGuids.ForEach(toSetGuid => stationHelper.Add(toSetGuid.Item1));
        });
        return visitedMain;
    }

    private static Dictionary<Tuple<Guid, int>, bool> InitVisited(List<Tuple<Guid, int>> stations)
    {
        Dictionary<Tuple<Guid, int>, bool> visited = new();
        stations.ForEach(station => visited.Add(station, false));
        return visited;
    }

    private static Tuple<Guid, int> CheckVisited(Dictionary<Tuple<Guid, int>, bool> visited)
    {
        List<Tuple<Guid, int>> stations = visited.Keys.ToList();
        foreach (var station in stations)
        {
            if (!visited[station]) return station;
        }
        return default;
    }

    private static Tuple<Guid, int> GetTuple(Dictionary<Tuple<Guid, int>, bool> visited, int stationNum)
    {
        foreach (var kvp in visited)
        {
            if (kvp.Key.Item2 == stationNum)
                return kvp.Key;
        }
        throw new InvalidProgramException("No such station number found");
    }
}
