using System.Collections.Generic;
using NUnit.Framework;

public class TrainManagerTests
{
    [TestCaseSource("TrainStubs")]
    public void TrainManager_AddTrain_TrainIsAdded(Train targetTrain)
    {
        TrainManager trainManager = TrainManagerInit();
        trainManager.AddTrain(targetTrain);
        Assert.IsTrue(trainManager.TrainList.Contains(targetTrain.TrainName));
    }

    [TestCaseSource("TrainStubs")]
    public void TrainManager_RemoveTrain_TrainIsRemoved(Train targetTrain)
    {
        TrainManager trainManager = TrainManagerInit();
        List<Train> trains = Trains;
        foreach (var train in trains)
        {
            trainManager.AddTrain(train);
        }

        Assert.IsTrue(trainManager.TrainList.Contains(targetTrain.TrainName));
        trainManager.RemoveTrain(targetTrain);
        Assert.IsFalse(trainManager.TrainList.Contains(targetTrain.TrainName));
    }

    private TrainManager TrainManagerInit() => new();

    private static List<TestCaseData> TrainStubs
    {
        get
        {
            List<Train> trains = Trains;
            List<TestCaseData> testCases = new();
            foreach (var train in trains)
            {
                testCases.Add(new TestCaseData(train));
            }
            return testCases;
        }
    }

    private static List<Train> Trains
    {
        get
        {
            TrainAttribute trainAttribute = new(
                new(0, 10, 10, 0),
                new(0.0, 100.0, 100.0, 0.5),
                new(0.0, 100.0, 100.0, 0.5),
                new(0.0, 100.0, 100.0, 0.5));
            return new List<Train>()
            {
                new Train("Train1", TrainType.Diesel, trainAttribute, new()),
                new Train("Train2", TrainType.Diesel, trainAttribute, new()),
                new Train("Train3", TrainType.Steam, trainAttribute, new())
            };
        }
    }
}
