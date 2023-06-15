using System;
using NUnit.Framework;

public class TrainAttributeTests
{
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
        Assert.AreEqual(trainAttribute.DoubleArithmetic(baseValue + increment), trainAttribute.Fuel.Rate);
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
        Assert.AreEqual(trainAttribute.DoubleArithmetic(baseValue + increment), trainAttribute.Fuel.UpperLimit);
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
        Assert.AreEqual(trainAttribute.DoubleArithmetic(baseValue + increment), trainAttribute.Durability.Rate);
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
        Assert.AreEqual(trainAttribute.DoubleArithmetic(baseValue + increment), trainAttribute.Durability.UpperLimit);
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
        Assert.AreEqual(trainAttribute.DoubleArithmetic(baseValue + increment), trainAttribute.Speed.UpperLimit);
    }

    [TestCase(0, -50)]
    [TestCase(double.MinValue, -1)]
    public void TrainAttribute_UpgradeSpeedLimit_SpeedLimitInvalid(double baseValue, double increment)
    {
        TrainAttribute trainAttribute = TrainAttributeInit(speedLimit: baseValue);
        Assert.Catch<ArgumentException>(() => trainAttribute.UpgradeSpeedLimit(increment));
    }

    private TrainAttribute TrainAttributeInit(
        int capacityLimit = 0,
        double fuelRate = 0.0,
        double fuelLimit = 0.0,
        double durabilityRate = 0.0,
        double durabilityLimit = 0.0,
        double speedLimit = 0.0)
    {
        TrainAttribute trainAttribute = new(
            new(0, capacityLimit, 0, 0),
            new(0, fuelLimit, 0, fuelRate),
            new(0, durabilityLimit, 0, durabilityRate),
            new(0, speedLimit, 0, 0));
        return trainAttribute;
    }
}
