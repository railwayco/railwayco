using NUnit.Framework;

public class TrainTests
{
    [Test]
    public void Train_Train_IsJsonSerialisedCorrectly()
    {
        Train train = TrainInit();
        train.CargoHelper.Add(System.Guid.NewGuid());

        string jsonString = GameDataManager.Serialize(train);
        Train trainToVerify = (Train)GameDataManager.Deserialize(typeof(Train), jsonString);

        Assert.AreEqual(train, trainToVerify);
    }

    [Test]
    public void Train_Clone_IsDeepCopy()
    {
        Train train = TrainInit();
        Train trainClone = (Train)train.Clone();

        trainClone.Attribute.Capacity.Amount = 9;
        trainClone.Attribute.Fuel.Amount = 50;
        trainClone.Attribute.Durability.Amount = 50;
        trainClone.Attribute.SetUnityStats(10, new(2, 3, 4), new(2, 3, 4, 5), TrainDirection.SOUTH);
        trainClone.CargoHelper.Add(System.Guid.NewGuid());

        Assert.AreNotEqual(train, trainClone);
    }

    private Train TrainInit()
    {
        TrainAttribute trainAttribute = new(
            new(0, 10, 5, 0),
            new(0, 100, 100, 5),
            new(0, 100, 100, 5),
            new(0, 10, 0, 0),
            new(1, 2, 3),
            new(1, 2, 3, 4),
            TrainDirection.NORTH);
        Train train = new("Train", TrainType.Steam, trainAttribute, new());
        return train;
    }
}
