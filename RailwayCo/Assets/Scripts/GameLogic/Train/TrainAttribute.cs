public class TrainAttribute : Arithmetic
{
    public Attribute<int> Capacity { get; private set; }
    public Attribute<double> Fuel { get; private set; }
    public Attribute<double> Durability { get; private set; }
    public Attribute<double> Speed { get; private set; }

    public TrainAttribute(
        Attribute<int> capacity,
        Attribute<double> fuel,
        Attribute<double> durability,
        Attribute<double> speed)
    {
        Capacity = capacity;
        Fuel = fuel;
        Durability = durability;
        Speed = speed;
    }

    public void UpgradeCapacityLimit(int capacityLimit)
    {
        if (capacityLimit < 0) throw new System.ArgumentException("Invalid capacity limit");
        Capacity.UpperLimit = IntAddition(Capacity.UpperLimit, capacityLimit);
    }

    public void UpgradeFuelRate(double fuelRate)
    {
        if (fuelRate < 0.0) throw new System.ArgumentException("Invalid fuel rate");
        Fuel.Rate = DoubleRangeCheck(Fuel.Rate + fuelRate);
        
    }

    public void UpgradeFuelLimit(double fuelLimit)
    {
        if (fuelLimit < 0.0) throw new System.ArgumentException("Invalid fuel limit");
        Fuel.UpperLimit = DoubleRangeCheck(Fuel.UpperLimit + fuelLimit);
    }

    public void UpgradeDurabilityRate(double durabilityRate)
    {
        if (durabilityRate < 0.0) throw new System.ArgumentException("Invalid durability rate");
        Durability.Rate = DoubleRangeCheck(Durability.Rate + durabilityRate);
    }

    public void UpgradeDurabilityLimit(double durabilityLimit)
    {
        if (durabilityLimit < 0.0) throw new System.ArgumentException("Invalid durability limit");
        Durability.UpperLimit = DoubleRangeCheck(Durability.UpperLimit + durabilityLimit);
    }

    public void UpgradeSpeedLimit(double speedLimit)
    {
        if (speedLimit < 0.0) throw new System.ArgumentException("Invalid speed limit");
        Speed.UpperLimit = DoubleRangeCheck(Speed.UpperLimit + speedLimit);
    }
}