using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.Fields)]
public class PlatformMaster : IEquatable<PlatformMaster>
{
    private DictHelper<Platform> PlatformDict { get; set; }
    private Dictionary<int, HashsetHelper> StationLookupDict { get; set; }
    private Dictionary<string, Guid> StationPlatformLookupDict { get; set; }

    public PlatformMaster()
    {
        PlatformDict = new();
        StationLookupDict = new();
        StationPlatformLookupDict = new();
    }

    public void AddPlatform(Platform platform)
    {
        Guid platformGuid = platform.Guid;
        int stationNum = platform.StationNum;

        PlatformDict.Add(platform.Guid, platform);
        if (!StationLookupDict.ContainsKey(stationNum))
            StationLookupDict.Add(stationNum, new());
        StationLookupDict[platform.StationNum].Add(platformGuid);

        int platformNum = platform.PlatformNum;
        string stationPlatformString = JoinStationPlatformNum(stationNum, platformNum);
        StationPlatformLookupDict.Add(stationPlatformString, platformGuid);
    }

    public Platform GetPlatform(Guid platform) => PlatformDict.GetObject(platform);

    public int GetPlatformStationNum(Guid platform) => GetPlatform(platform).StationNum;

    public Guid GetPlatformGuidByStationAndPlatformNum(int stationNum, int platformNum)
    {
        string stationPlatformString = JoinStationPlatformNum(stationNum, platformNum);
        StationPlatformLookupDict.TryGetValue(stationPlatformString, out Guid platformGuid);
        return platformGuid;
    }

    public void UnlockPlatform(Guid platform) => GetPlatform(platform).Unlock();
    public void LockPlatform(Guid platform) => GetPlatform(platform).Lock();
    public void OpenPlatform(Guid platform) => GetPlatform(platform).Open();
    public void ClosePlatform(Guid platform) => GetPlatform(platform).Close();


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

    /// <summary>
    /// Links a new track to the source platform
    /// </summary>
    /// <param name="source">Source station</param>
    /// <param name="destination">Destination platform</param>
    public void AddPlatformTrack(Guid source, Guid destination)
    {
        Track srcToDestTrack = new(destination, OperationalStatus.Locked);
        Platform sourcePlatform = GetPlatform(source);
        sourcePlatform.AddTrack(srcToDestTrack);

        Track destToSrcTrack = new(source, OperationalStatus.Locked);
        Platform destinationPlatform = GetPlatform(destination);
        destinationPlatform.AddTrack(destToSrcTrack);
    }

    public HashSet<Track> GetPlatformTracks(Guid platform) => GetPlatform(platform).GetTracks();

    public Track GetPlatformTrack(Guid source, Guid destination) => GetPlatform(source).GetTrack(destination);

    public void UnlockPlatformTrack(Guid source, Guid destination)
    {
        GetPlatformTrack(source, destination).Unlock();
        GetPlatformTrack(destination, source).Unlock();
    }
    public void LockPlatformTrack(Guid source, Guid destination)
    {
        GetPlatformTrack(source, destination).Lock();
        GetPlatformTrack(destination, source).Lock();
    }
    public void OpenPlatformTrack(Guid source, Guid destination)
    {
        GetPlatformTrack(source, destination).Open();
        GetPlatformTrack(destination, source).Open();
    }
    public void ClosePlatformTrack(Guid source, Guid destination)
    {
        GetPlatformTrack(source, destination).Close();
        GetPlatformTrack(destination, source).Close();
    }

    private string JoinStationPlatformNum(int stationNum, int platformNum)
    {
        string[] stationPlatformStringArr = { stationNum.ToString(), platformNum.ToString() };
        string stationPlatformString = string.Join('_', stationPlatformStringArr);
        return stationPlatformString;
    }

    public bool Equals(PlatformMaster other)
    {
        foreach (var guid in PlatformDict.GetAll())
        {
            if (!PlatformDict.GetObject(guid)
                             .Equals(other.PlatformDict.GetObject(guid)))
                return false;
        }

        foreach (var stationNum in StationLookupDict.Keys)
        {
            if (!StationLookupDict.GetValueOrDefault(stationNum)
                                  .Equals(other.StationLookupDict.GetValueOrDefault(stationNum)))
                return false;
        }

        foreach (var stationPlatformStr in StationPlatformLookupDict.Keys)
        {
            if (!StationPlatformLookupDict.GetValueOrDefault(stationPlatformStr)
                                          .Equals(other.StationPlatformLookupDict.GetValueOrDefault(stationPlatformStr)))
                return false;
        }

        return true;
    }
}
