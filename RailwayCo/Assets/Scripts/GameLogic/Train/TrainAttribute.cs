public class TrainAttribute
{
    public Attribute<IntRanged> Capacity { get; private set; }
    public Attribute<DoubleRanged> Fuel { get; private set; }
    public Attribute<DoubleRanged> Durability { get; private set; }
    public Attribute<DoubleRanged> Speed { get; private set; }

    public TrainAttribute(
        Attribute<IntRanged> capacity,
        Attribute<DoubleRanged> fuel,
        Attribute<DoubleRanged> durability,
        Attribute<DoubleRanged> speed)
    {
        Capacity = capacity;
        Fuel = fuel;
        Durability = durability;
        Speed = speed;
    }

    public void UpgradeCapacityLimit(int capacityLimit)
    {
        if (capacityLimit < 0) throw new System.ArgumentException("Invalid capacity limit");
        Capacity.UpperLimit += capacityLimit;
    }

    public void UpgradeFuelRate(double fuelRate)
    {
        if (fuelRate < 0) throw new System.ArgumentException("Invalid fuel rate");
        Fuel.Rate += fuelRate;
    }

    public void UpgradeFuelLimit(double fuelLimit)
    {
        if (fuelLimit < 0) throw new System.ArgumentException("Invalid fuel limit");
        Fuel.UpperLimit += fuelLimit;
    }

    public void UpgradeDurabilityRate(double durabilityRate)
    {
        if (durabilityRate < 0) throw new System.ArgumentException("Invalid durability rate");
        Durability.Rate += durabilityRate;
    }

    public void UpgradeDurabilityLimit(double durabilityLimit)
    {
        if (durabilityLimit < 0) throw new System.ArgumentException("Invalid durability limit");
        Durability.UpperLimit += durabilityLimit;
    }

    public void UpgradeSpeedLimit(double speedLimit)
    {
        if (speedLimit < 0) throw new System.ArgumentException("Invalid speed limit");
        Speed.UpperLimit += speedLimit;
    }
}