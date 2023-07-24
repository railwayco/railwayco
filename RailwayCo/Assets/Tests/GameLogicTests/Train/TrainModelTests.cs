using NUnit.Framework;
using UnityEngine;

public class TrainModelTests
{
    [Test]
    public void TrainModel_TrainModel_IsJsonSerialisedCorrectly()
    {
        TrainModel trainModel = TrainModelInit();

        string jsonString = GameDataManager.Serialize(trainModel);
        TrainModel trainModelToVerify = GameDataManager.Deserialize<TrainModel>(jsonString);

        Assert.AreEqual(trainModel, trainModelToVerify);
    }

    [Test]
    public void TrainModel_InitUnityStats_IsUnityStatsSet()
    {
        TrainModel trainModel = TrainModelInit();
        TrainAttribute trainAttribute = trainModel.Attribute;

        double oldMaxSpeed = trainAttribute.Speed.UpperLimit;
        Vector3 oldPosition = trainAttribute.Position;
        Quaternion oldRotation = trainAttribute.Rotation;
        MovementDirection oldDirection = trainAttribute.MovementDirection;
        MovementState oldState = trainAttribute.MovementState;

        double newMaxSpeed = 10;
        Vector3 newPosition = new(1, 2, 3);
        Quaternion newRotation = new(1, 2, 3, 4);
        MovementDirection newDirection = MovementDirection.South;
        MovementState newState = MovementState.Moving;

        trainModel.InitUnityStats(newMaxSpeed, newPosition, newRotation, newDirection, newState);
        trainAttribute = trainModel.Attribute;
        Assert.AreNotEqual(oldMaxSpeed, newMaxSpeed);
        Assert.AreEqual(trainAttribute.Speed.UpperLimit, newMaxSpeed);
        Assert.AreNotEqual(oldPosition, newPosition);
        Assert.AreEqual(trainAttribute.Position, newPosition);
        Assert.AreNotEqual(oldRotation, newRotation);
        Assert.AreEqual(trainAttribute.Rotation, newRotation);
        Assert.AreNotEqual(oldDirection, newDirection);
        Assert.AreEqual(trainAttribute.MovementDirection, newDirection);
        Assert.AreNotEqual(oldState, newState);
        Assert.AreEqual(trainAttribute.MovementState, newState);
    }

    [Test]
    public void TrainModel_Clone_IsDeepCopy()
    {
        TrainModel trainModel = TrainModelInit();
        TrainModel trainModelClone = (TrainModel)trainModel.Clone();

        trainModelClone.InitUnityStats(10,
                                       new(1, 2, 3),
                                       new(1, 2, 3, 4),
                                       MovementDirection.South,
                                       MovementState.Moving);

        Assert.AreNotEqual(trainModel, trainModelClone);
    }

    private TrainModel TrainModelInit()
    {
        TrainAttribute trainAttribute = new(new(0, 10, 5, 0),
                                            new(0, 100, 100, 5),
                                            new(0, 100, 100, 5),
                                            new(0, 4, 0, 0),
                                            new(),
                                            new(),
                                            MovementDirection.North,
                                            MovementState.Stationary);
        TrainModel trainModel = new(TrainType.Steam, trainAttribute);
        return trainModel;
    }
}
