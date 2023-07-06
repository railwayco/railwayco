using NUnit.Framework;

public class StationTests
{
    [Test]
    public void Station_Open_IsCorrectStatusSet()
    {
        Station station = StationInit(StationStatus.Locked);
        station.Open();
        Assert.AreEqual(StationStatus.Open, station.Type);
    }

    [Test]
    public void Station_Close_IsCorrectStatusSet()
    {
        Station station = StationInit(StationStatus.Locked);
        station.Close();
        Assert.AreEqual(StationStatus.Closed, station.Type);
    }

    [Test]
    public void Station_Lock_IsCorrectStatusSet()
    {
        Station station = StationInit(StationStatus.Open);
        station.Lock();
        Assert.AreEqual(StationStatus.Locked, station.Type);
    }

    [Test]
    public void Station_Unlock_IsCorrectStatusSet()
    {
        Station station = StationInit(StationStatus.Locked);
        station.Unlock();
        Assert.AreEqual(StationStatus.Open, station.Type);
    }

    private Station StationInit(
        StationStatus stationStatus = StationStatus.Open,
        int yardCapacityLimit = 0,
        int yardCapacityAmount = 0,
        UnityEngine.Vector3 position = new())
    {
        StationAttribute stationAttribute = new(
            new(0, yardCapacityLimit, yardCapacityAmount, 0),
            position);
        Station station = new("Station",
                              stationStatus,
                              stationAttribute,
                              new(),
                              new(),
                              new());
        return station;
    }
}
