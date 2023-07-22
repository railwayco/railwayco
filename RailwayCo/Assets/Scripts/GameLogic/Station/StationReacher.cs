using System;
using System.Collections.Generic;
using System.Linq;

public static class StationReacher
{
    public static void UnlinkStations(StationMaster stationMaster, Guid station1, Guid station2)
    {
        stationMaster.RemoveStationFromStation(station1, station2);
        stationMaster.RemoveStationFromStation(station2, station1);
    }

    public static void DisconnectStation(StationMaster stationMaster, Guid station)
    {
        List<Guid> affectedStations = stationMaster.GetObject(station).StationHelper.ToList();
        affectedStations.ForEach(affectedStation =>
        {
            stationMaster.RemoveStationFromStation(affectedStation, station);
        });
    }

    public static void Bfs(StationMaster stationMaster, PlatformMaster platformMaster)
    {
        List<int> stationNums = stationMaster.GetAllStationNum().ToList();
        if (stationNums.Count == 0)
            return;

        Dictionary<int, bool> visitedMain = InitVisited(stationNums);

        int startStation = CheckVisited(visitedMain);
        do
        {
            Dictionary<int, bool> visited = InitVisited(stationNums);
            visited[startStation] = true;
            visited = BfsHelper(platformMaster, visited, startStation);

            visitedMain = UpdateMainStructs(stationMaster, visitedMain, visited);
            startStation = CheckVisited(visitedMain);
        } while (startStation != default);
    }

    private static Dictionary<int, bool> BfsHelper(
        PlatformMaster platformMaster,
        Dictionary<int, bool> visited,
        int startStation)
    {
        Queue<int> traversalQueue = new();
        traversalQueue.Enqueue(startStation);

        while (traversalQueue.Count != 0)
        {
            int targetStationNum = traversalQueue.Dequeue();
            visited[targetStationNum] = true;

            List<Guid> platforms = platformMaster.GetPlatformsByStationNum(targetStationNum).ToList();
            platforms.ForEach(platform =>
            {
                HashSet<Track> tracks = platformMaster.GetPlatformTracks(platform);
                foreach (var track in tracks)
                {
                    if (track.Status == OperationalStatus.Locked) continue;

                    int neighbourNum = platformMaster.GetPlatformStationNum(track.Platform);
                    if (!visited.ContainsKey(neighbourNum))
                    {
                        // Program is still initialising this station and cannot be found
                        continue;
                    }

                    if (!visited.GetValueOrDefault(neighbourNum))
                        traversalQueue.Enqueue(neighbourNum);
                }
            });
        }
        return visited;
    }

    private static Dictionary<int, bool> UpdateMainStructs(
        StationMaster stationMaster,
        Dictionary<int, bool> visitedMain,
        Dictionary<int, bool> visited)
    {
        List<int> stationNums = new(visited.Keys.Where(stationNum => visited[stationNum]));
        stationNums.ForEach(stationNum =>
        {
            visitedMain[stationNum] = true;
            Guid stationGuid = stationMaster.GetStationGuid(stationNum);

            List<int> neighbours = new(stationNums);
            neighbours.Remove(stationNum);
            neighbours.ForEach(neighbour => 
            {
                Guid toSetGuid = stationMaster.GetStationGuid(neighbour);
                stationMaster.AddStationToStation(stationGuid, toSetGuid);
            });
        });
        return visitedMain;
    }

    private static Dictionary<int, bool> InitVisited(List<int> stations)
    {
        Dictionary<int, bool> visited = new();
        stations.ForEach(station => visited.Add(station, false));
        return visited;
    }

    private static int CheckVisited(Dictionary<int, bool> visited)
    {
        List<int> stations = visited.Keys.ToList();
        foreach (var station in stations)
        {
            if (!visited[station]) return station;
        }
        return default;
    }
}
