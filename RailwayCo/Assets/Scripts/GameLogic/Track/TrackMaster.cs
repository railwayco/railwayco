using System;
using System.Collections.Generic;

public class TrackMaster
{
    private Dictionary<Tuple<int, int>, Track> Collection { get; set; }

    public TrackMaster() => Collection = new();

    public void AddTrackPair(Track track)
    {
        AddTrack(track);
        Track oppTrack = track.GetEquivalentPair();
        AddTrack(oppTrack);
    }

    public void RemoveTrackPair(int srcStationNum, int destStationNum)
    {
        RemoveTrack(srcStationNum, destStationNum);
        RemoveTrack(destStationNum, srcStationNum);
    }

    public Track GetTrack(int srcStationNum, int destStationNum)
    {
        Tuple<int, int> key = new(srcStationNum, destStationNum);
        if (!Collection.ContainsKey(key)) return default;
        return Collection[key];
    }

    private void AddTrack(Track track)
    {
        Tuple<int, int> key = new(track.SrcStationNum, track.DestStationNum);
        Collection.Add(key, track);
    }

    private void RemoveTrack(int srcStationNum, int destStationNum)
    {
        Tuple<int, int> key = new(srcStationNum, destStationNum);
        Collection.Remove(key);
    }
}
