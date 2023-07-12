using NUnit.Framework;

public class StationTests
{
    [Test]
    public void Station_Station_IsJsonSerialisedCorrectly()
    {
        Station station = StationInit(OperationStatus.Locked, 10, 5, new(1, 2, 3));
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
        Station station = StationInit(OperationStatus.Locked);
        station.Open();
        Assert.AreEqual(OperationStatus.Open, station.Status);
    }

    [Test]
    public void Station_Close_IsCorrectStatusSet()
    {
        Station station = StationInit(OperationStatus.Locked);
        station.Close();
        Assert.AreEqual(OperationStatus.Closed, station.Status);
    }

    [Test]
    public void Station_Lock_IsCorrectStatusSet()
    {
        Station station = StationInit(OperationStatus.Open);
        station.Lock();
        Assert.AreEqual(OperationStatus.Locked, station.Status);
    }

    [Test]
    public void Station_Unlock_IsCorrectStatusSet()
    {
        Station station = StationInit(OperationStatus.Locked);
        station.Unlock();
        Assert.AreEqual(OperationStatus.Open, station.Status);
    }

    [Test]
    public void Station_Clone_IsDeepCopy()
    {
        Station station = StationInit(OperationStatus.Locked, 10, 5, new(1, 2, 3));
        Station stationClone = (Station)station.Clone();

        stationClone.Attribute.YardCapacity.Amount = 9;
        stationClone.StationHelper.Add(System.Guid.NewGuid());
        stationClone.TrainHelper.Add(System.Guid.NewGuid());
        stationClone.CargoHelper.Add(System.Guid.NewGuid());

        Assert.AreNotEqual(station, stationClone);
    }

    private Station StationInit(
        OperationStatus operationStatus = OperationStatus.Open,
        int yardCapacityLimit = 0,
        int yardCapacityAmount = 0,
        UnityEngine.Vector3 position = new())
    {
        StationAttribute stationAttribute = new(
            new(0, yardCapacityLimit, yardCapacityAmount, 0),
            position);
        Station station = new("Station",
                              operationStatus,
                              stationAttribute,
                              new(),
                              new(),
                              new());
        return station;
    }
}
