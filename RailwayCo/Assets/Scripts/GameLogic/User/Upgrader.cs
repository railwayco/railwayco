using System;

public class Upgrader : IEquatable<Upgrader>
{
    public int SkillPoint { get; private set; }

    public Upgrader(int skillPoint) => SkillPoint = skillPoint;

    public void AddSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new ArgumentException("Invalid skill points");
        SkillPoint = Arithmetic.IntAddition(SkillPoint, skillPoint);
    }

    public void RemoveSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new ArgumentException("Invalid skill points");
        SkillPoint = Arithmetic.IntSubtraction(SkillPoint, skillPoint);
    }

    public void UpgradeTrain(TrainAttribute trainAttribute, TrainUpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case TrainUpgradeType.Capacity:
                trainAttribute.Capacity.UpgradeLimit();
                break;
            case TrainUpgradeType.FuelLimit:
                trainAttribute.Fuel.UpgradeLimit();
                break;
            case TrainUpgradeType.FuelRate:
                trainAttribute.Fuel.UpgradeRate();
                break;
            case TrainUpgradeType.DurabilityLimit:
                trainAttribute.Durability.UpgradeLimit();
                break;
            case TrainUpgradeType.DurabilityRate:
                trainAttribute.Durability.UpgradeRate();
                break;
            case TrainUpgradeType.SpeedLimit:
                trainAttribute.Speed.UpgradeLimit();
                break;
            default:
                throw new ArgumentException("Unsupported TrainUpgradeType");
        }
    }

    public void UpgradeStation(StationAttribute stationAttribute, StationUpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case StationUpgradeType.YardCapacity:
                stationAttribute.YardCapacity.UpgradeLimit();
                break;
            default:
                throw new ArgumentException("Unsupported StationUpgradeType");
        }
    }

    public bool Equals(Upgrader other)
    {
        return SkillPoint == other.SkillPoint;
    }
}
