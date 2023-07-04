using NUnit.Framework;

public class TrackTests
{
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
        Track track = new(1, 2, 1, departDirection);
        return track;
    }
}
