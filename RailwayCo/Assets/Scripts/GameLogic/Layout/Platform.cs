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
    private HashSet<Track> Tracks { get; }

    [JsonConstructor]
    private Platform(string guid, int stationNum, int platformNum, HashSet<Track> tracks)
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

    public void AddTrack(Track track) => Tracks.Add(track);

    public HashSet<Track> GetTracks()
    {
        HashSet<Track> tracks = new();
        Tracks.ToList().ForEach(track => tracks.Add((Track)track.Clone()));
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
        return Guid.Equals(other.Guid)
            && StationNum == other.StationNum
            && PlatformNum == other.PlatformNum
            && Tracks.SetEquals(other.Tracks);
    }
}
