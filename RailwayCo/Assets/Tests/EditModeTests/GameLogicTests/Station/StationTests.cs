using NUnit.Framework;

public class StationTests
{
    [Test]
    public void Station_Station_IsJsonSerialisedCorrectly()
    {
        Station station = StationInit(StationStatus.Locked, 10, 5, new(1, 2, 3));
        station.StationHelper.Add(System.Guid.NewGuid());
        station.TrainHelper.Add(System.Guid.NewGuid());
        station.CargoHelper.Add(System.Guid.NewGuid());

        string jsonString = GameDataManager.Serialize(station);
        Station stationToVerify = GameDataManager.Deserialize<Station>(jsonString);

        Assert.AreEqual(station, stationToVerify);
    }
    
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

    [Test]
    public void Station_Clone_IsDeepCopy()
    {
        Station station = StationInit(StationStatus.Locked, 10, 5, new(1, 2, 3));
        Station stationClone = (Station)station.Clone();

        stationClone.Attribute.YardCapacity.Amount = 9;
        stationClone.StationHelper.Add(System.Guid.NewGuid());
        stationClone.TrainHelper.Add(System.Guid.NewGuid());
        stationClone.CargoHelper.Add(System.Guid.NewGuid());

        Assert.AreNotEqual(station, stationClone);
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
