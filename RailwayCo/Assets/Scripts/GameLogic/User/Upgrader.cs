using System;

public class Upgrader : Arithmetic
{
    public int SkillPoint { get; private set; }

    public Upgrader(int skillPoint) => SkillPoint = skillPoint;

    public void AddSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new ArgumentException("Invalid skill points");
        SkillPoint = IntAddition(SkillPoint, skillPoint);
    }

    public void RemoveSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new ArgumentException("Invalid skill points");
        SkillPoint = IntSubtraction(SkillPoint, skillPoint);
    }

    public void UpgradeTrain(TrainAttribute trainAttribute, TrainUpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case TrainUpgradeType.Capacity:
                {
                    trainAttribute.Capacity.UpgradeLimit();
                    break;
                }
            case TrainUpgradeType.FuelLimit:
                {
                    trainAttribute.Fuel.UpgradeLimit();
                    break;
                }
            case TrainUpgradeType.FuelRate:
                {
                    trainAttribute.Fuel.UpgradeRate();
                    break;
                }
            case TrainUpgradeType.DurabilityLimit:
                {
                    trainAttribute.Durability.UpgradeLimit();
                    break;
                }
            case TrainUpgradeType.DurabilityRate:
                {
                    trainAttribute.Durability.UpgradeRate();
                    break;
                }
            case TrainUpgradeType.SpeedLimit:
                {
                    trainAttribute.Speed.UpgradeLimit();
                    break;
                }
            default:
                throw new ArgumentException("Unsupported UpgradeType");
        }
    }

    public void UpgradeStation(StationAttribute stationAttribute, StationUpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case StationUpgradeType.YardCapacity:
                {
                    stationAttribute.YardCapacity.UpgradeLimit();
                    break;
                }
            default:
                throw new ArgumentException("Unsupported UpgradeType");
        }
    }

    private void CalculateUpgradeAmount()
    {

    }
}
