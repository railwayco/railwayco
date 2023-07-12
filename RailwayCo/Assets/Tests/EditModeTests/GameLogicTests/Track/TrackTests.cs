using NUnit.Framework;

public class TrackTests
{
    [Test]
    public void Track_Track_IsJsonSerialisedCorrectly()
    {
        Track track = TrackInit(DepartDirection.Right);

        string jsonString = GameDataManager.Serialize(track);
        Track trackToVerify = GameDataManager.Deserialize<Track>(jsonString);

        Assert.AreEqual(track, trackToVerify);
    }
    
    [TestCase(DepartDirection.Left, ExpectedResult = DepartDirection.Right)]
    [TestCase(DepartDirection.Right, ExpectedResult = DepartDirection.Left)]
    [TestCase(DepartDirection.Up, ExpectedResult = DepartDirection.Down)]
    [TestCase(DepartDirection.Down, ExpectedResult = DepartDirection.Up)]
    public DepartDirection Track_GetEquivalentPair_IsPairOpposite(DepartDirection departDirection)
    {
        Track track = TrackInit(departDirection);
        Track oppTrack = track.GetEquivalentPair();
        return oppTrack.DepartDirection;
    }

    private Track TrackInit(DepartDirection departDirection)
    {
        Track track = new(1, 1, 2, 2, 1, OperationalStatus.Open, departDirection);
        return track;
    }
}
