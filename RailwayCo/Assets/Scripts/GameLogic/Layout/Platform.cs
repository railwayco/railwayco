using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class Platform : IEquatable<Platform>
{
    public Guid Guid { get; }
    public int StationNum { get; }
    public int PlatformNum { get; }
    public OperationalStatus Status { get; private set; }

    [JsonProperty]
    private Dictionary<Guid, Track> Tracks { get; }

    [JsonConstructor]
    private Platform(string guid, int stationNum, int platformNum, string status, Dictionary<Guid, Track> tracks)
    {
        Guid = new(guid);
        StationNum = stationNum;
        PlatformNum = platformNum;
        Status = Enum.Parse<OperationalStatus>(status);
        Tracks = tracks;
    }
    
    public Platform(int stationNum, int platformNum)
    {
        Guid = Guid.NewGuid();
        StationNum = stationNum;
        PlatformNum = platformNum;
        Status = OperationalStatus.Locked;
        Tracks = new();
    }

    public void Open() => Status = OperationalStatus.Open;
    public void Close() => Status = OperationalStatus.Closed;
    public void Lock() => Status = OperationalStatus.Locked;
    public void Unlock() => Open();

    public void AddTrack(Track track) => Tracks.Add(track.Platform, track);

    public HashSet<Track> GetTracks()
    {
        HashSet<Track> tracks = new(Tracks.Values);
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
        foreach (var guid in Tracks.Keys)
        {
            if (!Tracks[guid].Equals(other.Tracks.GetValueOrDefault(guid)))
                return false;
        }

        return Guid.Equals(other.Guid)
            && StationNum == other.StationNum
            && PlatformNum == other.PlatformNum
            && Status == other.Status;
    }
}
