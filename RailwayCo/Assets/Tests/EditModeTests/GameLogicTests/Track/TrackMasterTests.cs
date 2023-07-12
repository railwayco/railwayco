using NUnit.Framework;

public class TrackMasterTests
{
    [Test]
    public void TrackMaster_AddTrackPair_AreTrackPairAdded()
    {
        TrackMaster trackMaster = TrackMasterInit();
        Track track = new(1, 1, 2, 2, 1, OperationalStatus.Open, DepartDirection.West);
        trackMaster.AddTrackPair(track, DepartDirection.North);
        Assert.IsTrue(trackMaster.GetTrack(1, 1, 2, 2) != default);
        Assert.IsTrue(trackMaster.GetTrack(2, 2, 1, 1) != default);
    }

    [Test]
    public void TrackMaster_RemoveTrackPair_AreTrackPairRemoved()
    {
        TrackMaster trackMaster = TrackMasterWithTracksInit();
        trackMaster.RemoveTrackPair(1, 2);
        Assert.IsTrue(trackMaster.GetTrack(1, 2) == default);
        Assert.IsTrue(trackMaster.GetTrack(2, 1) == default);
    }

    [Test]
    public void TrackMaster_GetTrack_TrackExists()
    {
        TrackMaster trackMaster = TrackMasterWithTracksInit();
        Assert.IsTrue(trackMaster.GetTrack(1, 2) != default);
    }

    private TrackMaster TrackMasterInit()
    {
        TrackMaster trackMaster = new();
        return trackMaster;
    }

    private TrackMaster TrackMasterWithTracksInit()
    {
        TrackMaster trackMaster = TrackMasterInit();
        Track track = new(1, 1, 2, 2, 1, OperationalStatus.Open, DepartDirection.West);
        trackMaster.AddTrackPair(track, DepartDirection.North);
        return trackMaster;
    }
}
