using System;
using System.Collections.Generic;

public class PlatformMaster
{
    public DictHelper<Platform> PlatformDict { get; private set; }
    public Dictionary<int, HashsetHelper> StationLookupDict { get; private set; }

    public PlatformMaster()
    {
        PlatformDict = new();
        StationLookupDict = new();
    }

    public void AddPlatform(Platform platform)
    {
        Guid platformGuid = platform.Guid;
        int stationNum = platform.StationNum;

        PlatformDict.Add(platform.Guid, platform);
        if (!StationLookupDict.ContainsKey(stationNum))
            StationLookupDict.Add(stationNum, new());
        StationLookupDict[platform.StationNum].Add(platformGuid);
    }

    public void RemovePlatform(Guid platform)
    {
        Platform platformObject = PlatformDict.GetObject(platform);
        int stationNum = platformObject.StationNum;
        StationLookupDict[stationNum].Remove(platform);
        PlatformDict.Remove(platform);
    }

    /// <summary>
    /// Get all platform guids associated with station number
    /// </summary>
    /// <param name="stationNum">Station number to query</param>
    /// <returns>Hashset of guids</returns>
    public HashSet<Guid> GetPlatformsByStationNum(int stationNum)
    {
        StationLookupDict.TryGetValue(stationNum, out HashsetHelper platforms);
        return platforms is null ? default : platforms.GetAll();
    }

    public Platform GetPlatform(Guid platform) => PlatformDict.GetObject(platform);

    /// <summary>
    /// Links a new track to the source platform
    /// </summary>
    /// <param name="source">Source station</param>
    /// <param name="destination">Destination platform</param>
    /// <param name="srcToDest">Depart direction of train travelling from source to destination</param>
    /// <param name="DestToSrc">Depart direction of train travelling from destination to source</param>
    public void AddPlatformTrack(Guid source, Guid destination, DepartDirection srcToDest, DepartDirection DestToSrc)
    {
        Track srcToDestTrack = new(destination, srcToDest, OperationalStatus.Locked);
        Platform sourcePlatform = GetPlatform(source);
        sourcePlatform.AddTrack(srcToDestTrack);

        Track destToSrcTrack = new(source, DestToSrc, OperationalStatus.Locked);
        Platform destinationPlatform = GetPlatform(destination);
        destinationPlatform.AddTrack(destToSrcTrack);
    }

    public HashSet<Track> GetPlatformTracks(Guid platform) => GetPlatform(platform).GetTracks();
}
