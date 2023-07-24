using System;
using UnityEngine;
using NUnit.Framework;

public class TrainAttributeTests
{
    [Test]
    public void TrainAttribute_TrainAttribute_IsJsonSerialisedCorrectly()
    {
        TrainAttribute trainAttribute = TrainAttributeInit(capacityAmount: 10,
                                                           fuelAmount: 100,
                                                           durabilityAmount: 100,
                                                           speed: 10,
                                                           position: new(1, 2, 3),
                                                           rotation: new(1, 2, 3, 4),
                                                           movementDirection: MovementDirection.North,
                                                           movementState: MovementState.Stationary);

        string jsonString = GameDataManager.Serialize(trainAttribute);
        TrainAttribute trainAttrbToVerify = GameDataManager.Deserialize<TrainAttribute>(jsonString);

        Assert.AreEqual(trainAttribute, trainAttrbToVerify);
    }
    
    [TestCase(0F, MovementDirection.North, MovementState.Moving)]
    [TestCase(float.MaxValue, MovementDirection.South, MovementState.Stationary)]
    public void TrainAttribute_SetUnityStats_UnityStatsSaved(
        float speed,
        MovementDirection movementDirection,
        MovementState movementState,
        Vector3 position = new(),
        Quaternion rotation = new())
    {
        TrainAttribute trainAttribute = TrainAttributeInit();
        trainAttribute.SetUnityStats(speed, position, rotation, movementDirection, movementState);

        Assert.AreEqual((double)speed, trainAttribute.Speed.Amount);
        Assert.AreEqual(position, trainAttribute.Position);
        Assert.AreEqual(rotation, trainAttribute.Rotation);
        Assert.AreEqual(movementDirection, trainAttribute.MovementDirection);
        Assert.AreEqual(movementState, trainAttribute.MovementState);
    }

    [TestCase(50, 50)]
    [TestCase(50, 0)]
    public void TrainAttribute_IsCapacityFull_CapacityAmountMaxed(int limit, int amount)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(capacityLimit: limit, capacityAmount: amount);
        if (amount >= limit)
            Assert.AreEqual(true, trainAttribute.IsCapacityFull());
        else
            Assert.AreEqual(false, trainAttribute.IsCapacityFull());
    }

    [TestCase(50)]
    public void TrainAttribute_AddToCapacity_CapacityAmountIncreased(int baseValue)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(capacityAmount: baseValue);
        trainAttribute.AddToCapacity();
        Assert.AreEqual(Arithmetic.IntAddition(baseValue, 1), trainAttribute.Capacity.Amount);
    }

    [TestCase(int.MaxValue)]
    public void TrainAttribute_AddToCapacity_CapacityAmountInvalid(int baseValue)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(capacityAmount: baseValue);
        Assert.Catch<ArithmeticException>(() => trainAttribute.AddToCapacity());
    }

    [TestCase(1)]
    public void TrainAttribute_RemoveFromCapacity_CapacityAmountDecreased(int baseValue)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(capacityAmount: baseValue);
        trainAttribute.RemoveFromCapacity();
        Assert.AreEqual(Arithmetic.IntSubtraction(baseValue, 1), trainAttribute.Capacity.Amount);
    }

    [TestCase(0)]
    public void TrainAttributee_RemoveFromCapacity_CapacityAmountInvalid(int baseValue)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(capacityAmount: baseValue);
        Assert.Catch<ArithmeticException>(() => trainAttribute.RemoveFromCapacity());
    }
    
    [TestCase(10.0, 5.5, ExpectedResult = true)]
    [TestCase(0.0, 10.5, ExpectedResult = false)]
    public bool TrainAttribute_BurnFuel_FuelAmountLogicCorrect(double fuelAmount, double fuelRate)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(fuelAmount: fuelAmount, fuelRate: fuelRate);
        return trainAttribute.BurnFuel();
    }

    [TestCase(0.0, 10.0, 5.0, ExpectedResult = true)]
    [TestCase(0.0, 10.0, 10.0, ExpectedResult = true)]
    [TestCase(7.5, 10.0, 5.0, ExpectedResult = true)]
    [TestCase(10.0, 10.0, 5.0, ExpectedResult = false)]
    public bool TrainAttribute_Refuel_FuelAmountLogicCorrect(double fuelAmount, double fuelLimit, double fuelRate)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(fuelAmount: fuelAmount,
                                                           fuelLimit: fuelLimit,
                                                           fuelRate: fuelRate);
        return trainAttribute.Refuel();
    }

    [TestCase(10.0, 5.5, ExpectedResult = true)]
    [TestCase(0.0, 10.5, ExpectedResult = false)]
    public bool TrainAttribute_DurabilityTear_DurabilityAmountLogicCorrect(
        double durabilityAmount,
        double durabilityRate)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(durabilityAmount: durabilityAmount,
                                                           durabilityRate: durabilityRate);
        return trainAttribute.DurabilityWear();
    }

    [TestCase(0.0, 10.0, 5.0, ExpectedResult = true)]
    [TestCase(0.0, 10.0, 10.0, ExpectedResult = true)]
    [TestCase(7.5, 10.0, 5.0, ExpectedResult = true)]
    [TestCase(10.0, 10.0, 5.0, ExpectedResult = false)]
    public bool TrainAttribute_DurabilityRepair_DurabilityAmountLogicCorrect(
        double durabilityAmount,
        double durabilityLimit,
        double durabilityRate)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(durabilityAmount: durabilityAmount,
                                                           durabilityLimit: durabilityLimit,
                                                           durabilityRate: durabilityRate);
        return trainAttribute.DurabilityRepair();
    }

    [Test]
    public void TrainAttribute_Clone_IsDeepCopy()
    {
        TrainAttribute trainAttribute = TrainAttributeInit(capacityAmount: 10,
                                                           fuelAmount: 100,
                                                           durabilityAmount: 100,
                                                           speed: 10,
                                                           position: new(1, 2, 3),
                                                           rotation: new(1, 2, 3, 4),
                                                           movementDirection: MovementDirection.North,
                                                           movementState: MovementState.Stationary);
        TrainAttribute trainAttributeClone = (TrainAttribute)trainAttribute.Clone();
        trainAttributeClone.Capacity.Amount = 50;
        trainAttributeClone.Fuel.Amount = 50;
        trainAttributeClone.Durability.Amount = 50;
        trainAttributeClone.SetUnityStats(50,
                                          new(2, 3, 4),
                                          new(12, 13, 14, 15),
                                          MovementDirection.South,
                                          MovementState.Moving);

        Assert.AreNotEqual(trainAttribute, trainAttributeClone);
    }

    private TrainAttribute TrainAttributeInit(
        int capacityAmount = 0,
        int capacityLimit = 0,
        double fuelAmount = 0.0,
        double fuelRate = 0.0,
        double fuelLimit = 0.0,
        double durabilityAmount = 0.0,
        double durabilityRate = 0.0,
        double durabilityLimit = 0.0,
        double speedLimit = 0.0,
        double speed = 0.0,
        Vector3 position = default,
        Quaternion rotation = default,
        MovementDirection movementDirection = MovementDirection.North,
        MovementState movementState = MovementState.Stationary)
    {
        TrainAttribute trainAttribute = new(
            new(0, capacityLimit, capacityAmount, 0),
            new(0, fuelLimit, fuelAmount, fuelRate),
            new(0, durabilityLimit, durabilityAmount, durabilityRate),
            new(0, speedLimit, speed, 0),
            position,
            rotation,
            movementDirection,
            movementState);
        return trainAttribute;
    }
}
