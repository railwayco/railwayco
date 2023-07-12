using NUnit.Framework;

public class PlatformMasterTests
{
    [Test]
    public void PlatformMaster_AddTrackPair_AreTrackPairAdded()
    {
        PlatformMaster platformMaster = PlatformMasterInit();
        Track track = new(1, 1, 2, 2, 1, OperationalStatus.Open, DepartDirection.West);
        platformMaster.AddTrackPair(track, DepartDirection.North);
        Assert.IsTrue(platformMaster.GetTrack(1, 1, 2, 2) != default);
        Assert.IsTrue(platformMaster.GetTrack(2, 2, 1, 1) != default);
    }

    [Test]
    public void PlatformMaster_RemoveTrackPair_AreTrackPairRemoved()
    {
        PlatformMaster platformMaster = PlatformMasterWithTracksInit();
        platformMaster.RemoveTrackPair(1, 1, 2, 2);
        Assert.IsTrue(platformMaster.GetTrack(1, 1, 2, 2) == default);
        Assert.IsTrue(platformMaster.GetTrack(2, 2, 1, 1) == default);
    }

    [Test]
    public void PlatformMaster_GetTrack_TrackExists()
    {
        PlatformMaster platformMaster = PlatformMasterWithTracksInit();
        Assert.IsTrue(platformMaster.GetTrack(1, 1, 2, 2) != default);
    }

    private PlatformMaster PlatformMasterInit()
    {
        PlatformMaster platformMaster = new();
        return platformMaster;
    }

    private PlatformMaster PlatformMasterWithTracksInit()
    {
        PlatformMaster platformMaster = PlatformMasterInit();
        Track track = new(1, 1, 2, 2, 1, OperationalStatus.Open, DepartDirection.West);
        platformMaster.AddTrackPair(track, DepartDirection.North);
        return platformMaster;
    }
}
