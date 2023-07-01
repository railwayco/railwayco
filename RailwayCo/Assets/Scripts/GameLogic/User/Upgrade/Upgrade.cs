using System;

public class Upgrade : Arithmetic
{
    public int SkillPoint { get; private set; }

    public Upgrade(int skillPoint) => SkillPoint = skillPoint;

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

    public void UpgradeTrain(TrainAttribute trainAttribute, UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.TrainCapacity:
                {
                    trainAttribute.Capacity.UpgradeLimit();
                    break;
                }
            case UpgradeType.TrainFuelLimit:
                {
                    trainAttribute.Fuel.UpgradeLimit();
                    break;
                }
            case UpgradeType.TrainFuelRate:
                {
                    trainAttribute.Fuel.UpgradeRate();
                    break;
                }
            case UpgradeType.TrainDurabilityLimit:
                {
                    trainAttribute.Durability.UpgradeLimit();
                    break;
                }
            case UpgradeType.TrainDurabilityRate:
                {
                    trainAttribute.Durability.UpgradeRate();
                    break;
                }
            case UpgradeType.TrainSpeedLimit:
                {
                    trainAttribute.Speed.UpgradeLimit();
                    break;
                }
            default:
                throw new ArgumentException("Unsupported UpgradeType");
        }
    }

    public void UpgradeStation(StationAttribute stationAttribute, UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.StationYardCapacity:
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
