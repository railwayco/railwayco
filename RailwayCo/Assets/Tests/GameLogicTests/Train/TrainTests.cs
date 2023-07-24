using NUnit.Framework;

public class TrainTests
{
    [Test]
    public void Train_Train_IsJsonSerialisedCorrectly()
    {
        Train train = TrainInit();
        train.CargoHelper.Add(System.Guid.NewGuid());

        string jsonString = GameDataManager.Serialize(train);
        Train trainToVerify = GameDataManager.Deserialize<Train>(jsonString);

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
        trainClone.Attribute.SetUnityStats(10, new(2, 3, 4), new(2, 3, 4, 5), MovementDirection.South);
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
            MovementDirection.North);
        TrainModel trainModel = new(TrainType.Steam, trainAttribute);
        Train train = new(trainModel);
        return train;
    }
}
