using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class Platform : IEquatable<Platform>
{
    public Guid Guid { get; }
    public int StationNum { get; }
    public int PlatformNum { get; }
    
    [JsonProperty]
    private DictHelper<Track> Tracks { get; }

    [JsonConstructor]
    private Platform(string guid, int stationNum, int platformNum, DictHelper<Track> tracks)
    {
        Guid = new(guid);
        StationNum = stationNum;
        PlatformNum = platformNum;
        Tracks = tracks;
    }
    
    public Platform(int stationNum, int platformNum)
    {
        Guid = Guid.NewGuid();
        StationNum = stationNum;
        PlatformNum = platformNum;
        Tracks = new();
    }

    public void AddTrack(Track track) => Tracks.Add(track.Platform, track);

    public HashSet<Track> GetTracks()
    {
        HashSet<Track> tracks = new();
        Tracks.GetAll().ToList().ForEach(track => tracks.Add((Track)Tracks.GetObject(track).Clone()));
        return tracks;
    }

    public Track GetTrack(Guid platform)
    {
        HashSet<Track> tracks = GetTracks();
        foreach (var track in tracks)
        {
            if (track.Platform == platform)
                return track;
        }
        return default;
    }

    public bool Equals(Platform other)
    {
        foreach (var guid in Tracks.GetAll())
        {
            Track mainTrack = Tracks.GetObject(guid);
            Track trackToVerify = other.Tracks.GetObject(guid);
            if (!mainTrack.Equals(trackToVerify))
                return false;
        }

        return Guid.Equals(other.Guid)
            && StationNum == other.StationNum
            && PlatformNum == other.PlatformNum;
    }
}
