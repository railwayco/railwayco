public class TrainAttribute : Arithmetic
{
    private Attribute<int> capacity;
    private Attribute<double> fuel;
    private Attribute<double> durability;
    private Attribute<double> speed;

    public Attribute<int> Capacity { get => capacity; private set => capacity = value; }
    public Attribute<double> Fuel { get => fuel; private set => fuel = value; }
    public Attribute<double> Durability { get => durability; private set => durability = value; }
    public Attribute<double> Speed { get => speed; private set => speed = value; }

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
        if (fuelRate < 0) throw new System.ArgumentException("Invalid fuel rate");
        Fuel.Rate = DoubleArithmetic(Fuel.Rate + fuelRate);
    }

    public void UpgradeFuelLimit(double fuelLimit)
    {
        if (fuelLimit < 0) throw new System.ArgumentException("Invalid fuel limit");
        Fuel.UpperLimit = DoubleArithmetic(Fuel.UpperLimit + fuelLimit);
    }

    public void UpgradeDurabilityRate(double durabilityRate)
    {
        if (durabilityRate < 0) throw new System.ArgumentException("Invalid durability rate");
        Durability.Rate = DoubleArithmetic(Durability.Rate + durabilityRate);
    }

    public void UpgradeDurabilityLimit(double durabilityLimit)
    {
        if (durabilityLimit < 0) throw new System.ArgumentException("Invalid durability limit");
        Durability.UpperLimit = DoubleArithmetic(Durability.UpperLimit + durabilityLimit);
    }

    public void UpgradeSpeedLimit(double speedLimit)
    {
        if (speedLimit < 0) throw new System.ArgumentException("Invalid speed limit");
        Speed.UpperLimit = DoubleArithmetic(Speed.UpperLimit + speedLimit);
    }
}