using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public class PlatformTests
{
    [Test]
    public void Platform_Platform_IsJsonSerialisedCorrectly()
    {
        Platform platform = PlatformInit();

        string jsonString = GameDataManager.Serialize(platform);
        Platform platformToVerify = GameDataManager.Deserialize<Platform>(jsonString);

        Assert.AreEqual(platform, platformToVerify);
    }

    [Test]
    public void Platform_Open_IsCorrectStatusSet()
    {
        Platform platform = PlatformInit();
        platform.Open();
        Assert.AreEqual(OperationalStatus.Open, platform.Status);
    }

    [Test]
    public void Platform_Close_IsCorrectStatusSet()
    {
        Platform platform = PlatformInit();
        platform.Close();
        Assert.AreEqual(OperationalStatus.Closed, platform.Status);
    }

    [Test]
    public void Platform_Lock_IsCorrectStatusSet()
    {
        Platform platform = PlatformInit();
        platform.Lock();
        Assert.AreEqual(OperationalStatus.Locked, platform.Status);
    }

    [Test]
    public void Platform_Unlock_IsCorrectStatusSet()
    {
        Platform platform = PlatformInit();
        platform.Unlock();
        Assert.AreEqual(OperationalStatus.Open, platform.Status);
    }

    [Test]
    public void Platform_AddTrack_TrackIsAdded()
    {
        Platform platform = PlatformInit();
        Track track = new(System.Guid.NewGuid(), DepartDirection.West, OperationalStatus.Open);

        platform.AddTrack(track);

        List<Track> tracks = platform.GetTracks().ToList();
        Track trackToVerify = tracks[0];
        Assert.AreEqual(track, trackToVerify);
    }

    [Test]
    public void Platform_GetTracks_TracksArePresent()
    {
        Platform platform = PlatformInit();
        Track track1 = new(System.Guid.NewGuid(), DepartDirection.West, OperationalStatus.Open);
        Track track2 = new(System.Guid.NewGuid(), DepartDirection.East, OperationalStatus.Open);

        platform.AddTrack(track1);
        platform.AddTrack(track2);
        List<Track> tracks = platform.GetTracks().ToList();
        foreach (var trackToVerify in tracks)
        {
            if (trackToVerify.Platform == track1.Platform)
                Assert.AreEqual(track1, trackToVerify);
            else if (trackToVerify.Platform == track2.Platform)
                Assert.AreEqual(track2, trackToVerify);
            else
                Assert.Fail();
        }
    }

    [Test]
    public void Platform_GetTrack_TrackExists()
    {
        Platform platform = PlatformInit();
        Track track = new(System.Guid.NewGuid(), DepartDirection.West, OperationalStatus.Open);

        platform.AddTrack(track);
        Track trackToVerify = platform.GetTrack(track.Platform);
        Assert.AreEqual(track, trackToVerify);
    }

    private Platform PlatformInit()
    {
        Platform platform = new(1, 1);
        return platform;
    }
}
