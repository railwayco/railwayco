using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class Platform
{
    public Guid Guid { get; }
    public int StationNum { get; }
    public int PlatformNum { get; }
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
}
