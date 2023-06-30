using System;
using UnityEngine;
using NUnit.Framework;

public class TrainAttributeTests
{
    [TestCase(0.0F, TrainDirection.NORTH)]
    [TestCase(float.MaxValue, TrainDirection.SOUTH)]
    public void TrainAttribute_SetUnityStats_UnityStatsSaved(
        float speed,
        TrainDirection direction,
        Vector3 position = new(),
        Quaternion rotation = new())
    {
        TrainAttribute trainAttribute = TrainAttributeInit();
        trainAttribute.SetUnityStats(speed, position, rotation, direction);

        Assert.AreEqual((double)speed, trainAttribute.Speed.Amount);
        Assert.AreEqual(position, trainAttribute.Position);
        Assert.AreEqual(rotation, trainAttribute.Rotation);
        Assert.AreEqual(direction, trainAttribute.Direction);
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
        Assert.AreEqual(trainAttribute.IntAddition(baseValue, 1), trainAttribute.Capacity.Amount);
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
        Assert.AreEqual(trainAttribute.IntSubtraction(baseValue, 1), trainAttribute.Capacity.Amount);
    }

    [TestCase(0)]
    public void TrainAttribute_RemoveFromCapacity_CapacityAmountInvalid(int baseValue)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(capacityAmount: baseValue);
        Assert.Catch<ArithmeticException>(() => trainAttribute.RemoveFromCapacity());
    }

    private TrainAttribute TrainAttributeInit(
        int capacityAmount = 0,
        int capacityLimit = 0,
        double fuelRate = 0.0,
        double fuelLimit = 0.0,
        double durabilityRate = 0.0,
        double durabilityLimit = 0.0,
        double speedLimit = 0.0,
        double speed = 0.0,
        Vector3 position = default,
        Quaternion rotation = default,
        TrainDirection direction = TrainDirection.NORTH)
    {
        TrainAttribute trainAttribute = new(
            new(0, capacityLimit, capacityAmount, 0),
            new(0, fuelLimit, 0, fuelRate),
            new(0, durabilityLimit, 0, durabilityRate),
            new(0, speedLimit, 0, 0),
            position,
            rotation,
            direction);
        return trainAttribute;
    }
}
