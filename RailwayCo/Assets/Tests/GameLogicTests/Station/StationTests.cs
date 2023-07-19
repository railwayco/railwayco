using NUnit.Framework;

public class StationTests
{
    [Test]
    public void Station_Station_IsJsonSerialisedCorrectly()
    {
        Station station = StationInit(10, 5);
        station.StationHelper.Add(System.Guid.NewGuid());
        station.TrainHelper.Add(System.Guid.NewGuid());
        station.StationCargoHelper.Add(System.Guid.NewGuid());
        station.YardCargoHelper.Add(System.Guid.NewGuid());

        string jsonString = GameDataManager.Serialize(station);
        Station stationToVerify = GameDataManager.Deserialize<Station>(jsonString);

        Assert.AreEqual(station, stationToVerify);
    }

    [Test]
    public void Station_Clone_IsDeepCopy()
    {
        Station station = StationInit(10, 5);
        Station stationClone = (Station)station.Clone();

        stationClone.Attribute.YardCapacity.Amount = 9;
        stationClone.StationHelper.Add(System.Guid.NewGuid());
        stationClone.TrainHelper.Add(System.Guid.NewGuid());
        stationClone.StationCargoHelper.Add(System.Guid.NewGuid());
        stationClone.YardCargoHelper.Add(System.Guid.NewGuid());

        Assert.AreNotEqual(station, stationClone);
    }

    private Station StationInit(
        int yardCapacityLimit = 0,
        int yardCapacityAmount = 0)
    {
        StationAttribute stationAttribute = new(
            new(0, yardCapacityLimit, yardCapacityAmount, 0));
        Station station = new(1,
                              stationAttribute,
                              new(),
                              new(),
                              new(),
                              new());
        return station;
    }
}
