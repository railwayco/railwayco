﻿using System;
using System.Collections.Generic;
using System.Linq;

public class PlatformMaster : IPlayfab
{
    private Dictionary<Guid, Platform> PlatformDict { get; set; }
    private Dictionary<int, HashSet<Guid>> StationLookupDict { get; set; }
    private Dictionary<string, Guid> StationPlatformLookupDict { get; set; }

    public PlatformMaster()
    {
        PlatformDict = new();
        StationLookupDict = new();
        StationPlatformLookupDict = new();
    }

    /// <summary>
    /// Initialises all the platforms and tracks data
    /// Modify this according to how they are set out on the map
    /// </summary>
    public void InitPlatformsAndTracks()
    {
        // Create each platform in PlatformMaster
        // 7 stations, 2 platforms each
        for (int i = 1; i <= 7; i++)
        {
            for (int j = 1; j <= 2; j++)
            {
                Platform platform = new(i, j);
                AddPlatform(platform);
            }
        }

        // Link all the different platforms together using Track
        Guid platform_1_1 = GetPlatformGuidByStationAndPlatformNum(1, 1);
        Guid platform_1_2 = GetPlatformGuidByStationAndPlatformNum(1, 2);

        Guid platform_2_1 = GetPlatformGuidByStationAndPlatformNum(2, 1);
        Guid platform_2_2 = GetPlatformGuidByStationAndPlatformNum(2, 2);

        Guid platform_3_1 = GetPlatformGuidByStationAndPlatformNum(3, 1);
        Guid platform_3_2 = GetPlatformGuidByStationAndPlatformNum(3, 2);

        Guid platform_4_1 = GetPlatformGuidByStationAndPlatformNum(4, 1);
        Guid platform_4_2 = GetPlatformGuidByStationAndPlatformNum(4, 2);

        Guid platform_5_1 = GetPlatformGuidByStationAndPlatformNum(5, 1);
        Guid platform_5_2 = GetPlatformGuidByStationAndPlatformNum(5, 2);

        Guid platform_6_1 = GetPlatformGuidByStationAndPlatformNum(6, 1);
        Guid platform_6_2 = GetPlatformGuidByStationAndPlatformNum(6, 2);

        Guid platform_7_1 = GetPlatformGuidByStationAndPlatformNum(7, 1);
        Guid platform_7_2 = GetPlatformGuidByStationAndPlatformNum(7, 2);

        // Line A
        AddPlatformTrack(platform_1_1, platform_2_1);
        AddPlatformTrack(platform_1_1, platform_6_1);

        // Line B
        AddPlatformTrack(platform_2_2, platform_7_2);
        AddPlatformTrack(platform_2_2, platform_3_1);

        // Line C
        AddPlatformTrack(platform_6_2, platform_7_1);

        // Line D
        AddPlatformTrack(platform_3_2, platform_4_2);
        AddPlatformTrack(platform_4_2, platform_5_2);

        // Line E
        AddPlatformTrack(platform_1_2, platform_4_1);
        AddPlatformTrack(platform_4_1, platform_5_1);

        // UNLOCKS
        UnlockPlatform(platform_1_1);
        UnlockPlatform(platform_6_1);
        UnlockPlatformTrack(platform_1_1, platform_6_1);
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

    public Platform GetPlatform(Guid platform) => PlatformDict.GetValueOrDefault(platform);

    public int GetPlatformStationNum(Guid platform) => GetPlatform(platform).StationNum;

    public Guid GetPlatformGuidByStationAndPlatformNum(int stationNum, int platformNum)
    {
        string stationPlatformString = JoinStationPlatformNum(stationNum, platformNum);
        return StationPlatformLookupDict.GetValueOrDefault(stationPlatformString);
    }

    public HashSet<int> GetPlatformNeighbours(Guid platform)
    {
        List<Track> tracks = GetPlatformTracks(platform).ToList();
        HashSet<int> stationNums = new();
        tracks.ForEach(track =>
        {
            int stationNum = GetPlatform(track.Platform).StationNum;
            stationNums.Add(stationNum);
        });
        return stationNums;
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
    public HashSet<Guid> GetPlatformsByStationNum(int stationNum) => StationLookupDict.GetValueOrDefault(stationNum);

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

    public string SendDataToPlayfab() => GameDataManager.Serialize(PlatformDict);

    public void SetDataFromPlayfab(string data)
    {
        PlatformDict = GameDataManager.Deserialize<Dictionary<Guid, Platform>>(data);

        StationLookupDict = new();
        StationPlatformLookupDict = new();
        foreach (var keyValuePair in PlatformDict)
        {
            Guid platformGuid = keyValuePair.Key;
            Platform platform = keyValuePair.Value;

            int stationNum = platform.StationNum;
            if (!StationLookupDict.ContainsKey(stationNum))
                StationLookupDict.Add(stationNum, new());
            StationLookupDict[stationNum].Add(platformGuid);

            int platformNum = platform.PlatformNum;
            string stationPlatformString = JoinStationPlatformNum(stationNum, platformNum);
            StationPlatformLookupDict.Add(stationPlatformString, platformGuid);
        }
    }
}
