using System;
using NUnit.Framework;

public class UpgraderTests
{
    [Test]
    public void Upgrader_Upgrader_IsJsonSerialisedCorrectly()
    {
        Upgrader upgrader = UpgraderInit(100);

        string jsonString = GameDataManager.Serialize(upgrader);
        Upgrader upgraderToVerify = GameDataManager.Deserialize<Upgrader>(jsonString);

        Assert.AreEqual(upgraderToVerify, upgrader);
    }
    
    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void Upgrader_AddSkillPoint_SkillPointIncreased(int basePoint, int skillPoint)
    {
        Upgrader upgrader = UpgraderInit(basePoint);
        upgrader.AddSkillPoint(skillPoint);
        Assert.AreEqual(Arithmetic.IntAddition(basePoint, skillPoint), upgrader.SkillPoint);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void Upgrader_AddSkillPoint_SkillPointInvalid(int basePoint, int skillPoint)
    {
        Upgrader upgrader = UpgraderInit(basePoint);
        Assert.Catch<ArgumentException>(() => upgrader.AddSkillPoint(skillPoint));
    }

    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void Upgrader_RemoveSkillPoint_SkillPointDecreased(int basePoint, int skillPoint)
    {
        Upgrader upgrader = UpgraderInit(basePoint);
        upgrader.RemoveSkillPoint(skillPoint);
        Assert.AreEqual(Arithmetic.IntSubtraction(basePoint, skillPoint), upgrader.SkillPoint);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void Upgrader_RemoveSkillPoint_SkillPointInvalid(int basePoint, int skillPoint)
    {
        Upgrader upgrader = UpgraderInit(basePoint);
        Assert.Catch<ArgumentException>(() => upgrader.RemoveSkillPoint(skillPoint));
    }

    [Test]
    public void Upgrader_UpgradeTrain_CapacityUpgraded()
    {
        Upgrader upgrader = UpgraderInit(10);
        TrainUpgradeType trainUpgradeType = TrainUpgradeType.Capacity;
        int capacityLimit = 10;
        TrainAttribute trainAttribute = new(
            new(0, capacityLimit, 0, 0),
            new(0, 0, 0, 0),
            new(0, 0, 0, 0),
            new(0, 0, 0, 0),
            new(),
            new(),
            MovementDirection.North,
            MovementState.Stationary);

        upgrader.UpgradeTrain(trainAttribute, trainUpgradeType);
        Assert.AreNotSame(capacityLimit, trainAttribute.Capacity.UpperLimit);
    }

    [Test]
    public void Upgrader_UpgradeTrain_FuelRateUpgraded()
    {
        Upgrader upgrader = UpgraderInit(10);
        TrainUpgradeType trainUpgradeType = TrainUpgradeType.FuelRate;
        double fuelRate = 20.0;
        TrainAttribute trainAttribute = new(
            new(0, 0, 0, 0),
            new(0, 0, 0, fuelRate),
            new(0, 0, 0, 0),
            new(0, 0, 0, 0),
            new(),
            new(),
            MovementDirection.North,
            MovementState.Stationary);

        upgrader.UpgradeTrain(trainAttribute, trainUpgradeType);
        Assert.AreNotSame(fuelRate, trainAttribute.Fuel.Rate);
    }

    [Test]
    public void Upgrader_UpgradeTrain_FuelLimitUpgraded()
    {
        Upgrader upgrader = UpgraderInit(10);
        TrainUpgradeType trainUpgradeType = TrainUpgradeType.FuelLimit;
        double fuelLimit = 100.0;
        TrainAttribute trainAttribute = new(
            new(0, 0, 0, 0),
            new(0, fuelLimit, 0, 0),
            new(0, 0, 0, 0),
            new(0, 0, 0, 0),
            new(),
            new(),
            MovementDirection.North,
            MovementState.Stationary);

        upgrader.UpgradeTrain(trainAttribute, trainUpgradeType);
        Assert.AreNotSame(fuelLimit, trainAttribute.Fuel.UpperLimit);
    }

    [Test]
    public void Upgrader_UpgradeTrain_DurabilityRateUpgraded()
    {
        Upgrader upgrader = UpgraderInit(10);
        TrainUpgradeType trainUpgradeType = TrainUpgradeType.DurabilityRate;
        double durabilityRate = 20.0;
        TrainAttribute trainAttribute = new(
            new(0, 0, 0, 0),
            new(0, 0, 0, 0),
            new(0, 0, 0, durabilityRate),
            new(0, 0, 0, 0),
            new(),
            new(),
            MovementDirection.North,
            MovementState.Stationary);

        upgrader.UpgradeTrain(trainAttribute, trainUpgradeType);
        Assert.AreNotSame(durabilityRate, trainAttribute.Durability.Rate);
    }

    [Test]
    public void Upgrader_UpgradeTrain_DurabilityLimitUpgraded()
    {
        Upgrader upgrader = UpgraderInit(10);
        TrainUpgradeType trainUpgradeType = TrainUpgradeType.DurabilityLimit;
        double durabilityLimit = 100.0;
        TrainAttribute trainAttribute = new(
            new(0, 0, 0, 0),
            new(0, 0, 0, 0),
            new(0, durabilityLimit, 0, 0),
            new(0, 0, 0, 0),
            new(),
            new(),
            MovementDirection.North,
            MovementState.Stationary);

        upgrader.UpgradeTrain(trainAttribute, trainUpgradeType);
        Assert.AreNotSame(durabilityLimit, trainAttribute.Durability.UpperLimit);
    }

    [Test]
    public void Upgrader_UpgradeTrain_SpeedLimitUpgraded()
    {
        Upgrader upgrader = UpgraderInit(10);
        TrainUpgradeType trainUpgradeType = TrainUpgradeType.SpeedLimit;
        double speedLimit = 5.0;
        TrainAttribute trainAttribute = new(
            new(0, 0, 0, 0),
            new(0, 0, 0, 0),
            new(0, 0, 0, 0),
            new(0, speedLimit, 0, 0),
            new(),
            new(),
            MovementDirection.North,
            MovementState.Stationary);

        upgrader.UpgradeTrain(trainAttribute, trainUpgradeType);
        Assert.AreNotSame(speedLimit, trainAttribute.Speed.UpperLimit);
    }

    [Test]
    public void Upgrader_UpgradeStation_YardCapacityUpgraded()
    {
        Upgrader upgrader = UpgraderInit(10);
        StationAttribute stationAttribute = new(new(0, 10, 0, 0));

        StationUpgradeType stationUpgradeType = StationUpgradeType.YardCapacity;
        upgrader.UpgradeStation(stationAttribute, stationUpgradeType);
        Assert.AreNotSame(10, stationAttribute.YardCapacity.UpperLimit);
    }

    private Upgrader UpgraderInit(int skillPoint)
    {
        Upgrader upgrader = new(skillPoint);
        return upgrader;
    }
}
