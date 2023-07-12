using System;
using Newtonsoft.Json;

public class Platform
{
    public Guid Guid { get; }
    public int StationNum { get; }
    public int PlatformNum { get; }
    public Track HeadLink { get; private set; }
    public Track TailLink { get; private set; }

    [JsonConstructor]
    private Platform(string guid, int stationNum, int platformNum, Track headLink, Track tailLink)
    {
        Guid = new(guid);
        StationNum = stationNum;
        PlatformNum = platformNum;
        HeadLink = headLink;
        TailLink = tailLink;
    }
    
    public Platform(int stationNum, int platformNum)
    {
        Guid = Guid.NewGuid();
        StationNum = stationNum;
        PlatformNum = platformNum;
        HeadLink = default;
        TailLink = default;
    }

    /// <summary>
    /// Links a new track to the head of the platform, 
    /// where head is the right side of the platform.
    /// </summary>
    public void AddHeadLink(Track track) => HeadLink = track;

    /// <summary>
    /// Links a new track to the tail of the platform, 
    /// where tail is the left side of the platform.
    /// </summary>
    public void AddTailLink(Track track) => TailLink = track;
}
