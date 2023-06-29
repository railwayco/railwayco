using System;
using UnityEngine;
using NUnit.Framework;

public class TrainAttributeTests
{
    [TestCase(0F, TrainDirection.NORTH)]
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
        return trainAttribute.DurabilityTear();
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

    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void TrainAttribute_UpgradeCapacityLimit_CapacityLimitIncreased(int baseValue, int increment)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(capacityLimit: baseValue);
        trainAttribute.UpgradeCapacityLimit(increment);
        Assert.AreEqual(trainAttribute.IntAddition(baseValue, increment), trainAttribute.Capacity.UpperLimit);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void TrainAttribute_UpgradeCapacityLimit_CapacityLimitInvalid(int baseValue, int increment)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(capacityLimit: baseValue);
        Assert.Catch<ArgumentException>(() => trainAttribute.UpgradeCapacityLimit(increment));
    }

    [TestCase(0, 50)]
    [TestCase(double.MaxValue, 1)]
    public void TrainAttribute_UpgradeFuelRate_FuelRateIncreased(double baseValue, double increment)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(fuelRate: baseValue);
        trainAttribute.UpgradeFuelRate(increment);
        Assert.AreEqual(trainAttribute.DoubleRangeCheck(baseValue + increment), trainAttribute.Fuel.Rate);
    }

    [TestCase(0, -50)]
    [TestCase(double.MinValue, -1)]
    public void TrainAttribute_UpgradeFuelRate_FuelRateInvalid(double baseValue, double increment)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(fuelRate: baseValue);
        Assert.Catch<ArgumentException>(() => trainAttribute.UpgradeFuelRate(increment));
    }

    [TestCase(0, 50)]
    [TestCase(double.MaxValue, 1)]
    public void TrainAttribute_UpgradeFuelLimit_FuelLimitIncreased(double baseValue, double increment)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(fuelLimit: baseValue);
        trainAttribute.UpgradeFuelLimit(increment);
        Assert.AreEqual(trainAttribute.DoubleRangeCheck(baseValue + increment), trainAttribute.Fuel.UpperLimit);
    }

    [TestCase(0, -50)]
    [TestCase(double.MinValue, -1)]
    public void TrainAttribute_UpgradeFuelLimit_FuelLimitInvalid(double baseValue, double increment)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(fuelLimit: baseValue);
        Assert.Catch<ArgumentException>(() => trainAttribute.UpgradeFuelLimit(increment));
    }

    [TestCase(0, 50)]
    [TestCase(double.MaxValue, 1)]
    public void TrainAttribute_UpgradeDurabilityRate_DurabilityRateIncreased(double baseValue, double increment)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(durabilityRate: baseValue);
        trainAttribute.UpgradeDurabilityRate(increment);
        Assert.AreEqual(trainAttribute.DoubleRangeCheck(baseValue + increment), trainAttribute.Durability.Rate);
    }

    [TestCase(0, -50)]
    [TestCase(double.MinValue, -1)]
    public void TrainAttribute_UpgradeDurabilityRate_DurabilityRateInvalid(double baseValue, double increment)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(durabilityRate: baseValue);
        Assert.Catch<ArgumentException>(() => trainAttribute.UpgradeDurabilityRate(increment));
    }

    [TestCase(0, 50)]
    [TestCase(double.MaxValue, 1)]
    public void TrainAttribute_UpgradeDurabilityLimit_DurabilityLimitIncreased(double baseValue, double increment)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(durabilityLimit: baseValue);
        trainAttribute.UpgradeDurabilityLimit(increment);
        Assert.AreEqual(trainAttribute.DoubleRangeCheck(baseValue + increment), trainAttribute.Durability.UpperLimit);
    }

    [TestCase(0, -50)]
    [TestCase(double.MinValue, -1)]
    public void TrainAttribute_UpgradeDurabilityLimit_DurabilityLimitInvalid(double baseValue, double increment)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(durabilityLimit: baseValue);
        Assert.Catch<ArgumentException>(() => trainAttribute.UpgradeDurabilityLimit(increment));
    }

    [TestCase(0, 50)]
    [TestCase(double.MaxValue, 1)]
    public void TrainAttribute_UpgradeSpeedLimit_SpeedLimitIncreased(double baseValue, double increment)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(speedLimit: baseValue);
        trainAttribute.UpgradeSpeedLimit(increment);
        Assert.AreEqual(trainAttribute.DoubleRangeCheck(baseValue + increment), trainAttribute.Speed.UpperLimit);
    }

    [TestCase(0, -50)]
    [TestCase(double.MinValue, -1)]
    public void TrainAttribute_UpgradeSpeedLimit_SpeedLimitInvalid(double baseValue, double increment)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(speedLimit: baseValue);
        Assert.Catch<ArgumentException>(() => trainAttribute.UpgradeSpeedLimit(increment));
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
        TrainDirection direction = TrainDirection.NORTH)
    {
        TrainAttribute trainAttribute = new(
            new(0, capacityLimit, capacityAmount, 0),
            new(0, fuelLimit, fuelAmount, fuelRate),
            new(0, durabilityLimit, durabilityAmount, durabilityRate),
            new(0, speedLimit, speed, 0),
            position,
            rotation,
            direction);
        return trainAttribute;
    }
}
