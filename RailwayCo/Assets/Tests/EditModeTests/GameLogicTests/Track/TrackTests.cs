using NUnit.Framework;

public class TrackTests
{
    [Test]
    public void Track_Track_IsJsonSerialisedCorrectly()
    {
        Track track = TrackInit();

        string jsonString = GameDataManager.Serialize(track);
        Track trackToVerify = GameDataManager.Deserialize<Track>(jsonString);

        Assert.AreEqual(track, trackToVerify);
    }

    [Test]
    public void Track_Open_IsCorrectStatusSet()
    {
        Track track = TrackInit();
        track.Open();
        Assert.AreEqual(OperationalStatus.Open, track.Status);
    }

    [Test]
    public void Track_Close_IsCorrectStatusSet()
    {
        Track track = TrackInit();
        track.Close();
        Assert.AreEqual(OperationalStatus.Closed, track.Status);
    }

    [Test]
    public void Track_Lock_IsCorrectStatusSet()
    {
        Track track = TrackInit();
        track.Lock();
        Assert.AreEqual(OperationalStatus.Locked, track.Status);
    }

    [Test]
    public void Track_Unlock_IsCorrectStatusSet()
    {
        Track track = TrackInit();
        track.Unlock();
        Assert.AreEqual(OperationalStatus.Open, track.Status);
    }

    [Test]
    public void Track_Clone_IsDeepCopy()
    {
        Track track = TrackInit();
        Track trackClone = (Track)track.Clone();

        trackClone.Close();

        Assert.AreNotEqual(track, trackClone);
    }

    private Track TrackInit()
    {
        Track track = new(System.Guid.NewGuid(), DepartDirection.West, OperationalStatus.Open);
        return track;
    }
}
