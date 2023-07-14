using NUnit.Framework;

public class StationTests
{
    [Test]
    public void Station_Station_IsJsonSerialisedCorrectly()
    {
        Station station = StationInit(OperationalStatus.Locked, 10, 5);
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
        Station station = StationInit(OperationalStatus.Locked);
        station.Open();
        Assert.AreEqual(OperationalStatus.Open, station.Status);
    }

    [Test]
    public void Station_Close_IsCorrectStatusSet()
    {
        Station station = StationInit(OperationalStatus.Locked);
        station.Close();
        Assert.AreEqual(OperationalStatus.Closed, station.Status);
    }

    [Test]
    public void Station_Lock_IsCorrectStatusSet()
    {
        Station station = StationInit(OperationalStatus.Open);
        station.Lock();
        Assert.AreEqual(OperationalStatus.Locked, station.Status);
    }

    [Test]
    public void Station_Unlock_IsCorrectStatusSet()
    {
        Station station = StationInit(OperationalStatus.Locked);
        station.Unlock();
        Assert.AreEqual(OperationalStatus.Open, station.Status);
    }

    [Test]
    public void Station_Clone_IsDeepCopy()
    {
        Station station = StationInit(OperationalStatus.Locked, 10, 5);
        Station stationClone = (Station)station.Clone();

        stationClone.Attribute.YardCapacity.Amount = 9;
        stationClone.StationHelper.Add(System.Guid.NewGuid());
        stationClone.TrainHelper.Add(System.Guid.NewGuid());
        stationClone.CargoHelper.Add(System.Guid.NewGuid());

        Assert.AreNotEqual(station, stationClone);
    }

    private Station StationInit(
        OperationalStatus operationStatus = OperationalStatus.Open,
        int yardCapacityLimit = 0,
        int yardCapacityAmount = 0)
    {
        StationAttribute stationAttribute = new(
            new(0, yardCapacityLimit, yardCapacityAmount, 0));
        Station station = new(1,
                              operationStatus,
                              stationAttribute,
                              new(),
                              new(),
                              new());
        return station;
    }
}
