public class TrainAttribute
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

    public void UpgradeCapacityLimit(int capacityLimit) => Capacity.UpperLimit += capacityLimit;
    public void UpgradeFuelRate(double fuelRate) => Fuel.Rate += fuelRate;
    public void UpgradeFuelLimit(double fuelLimit) => Fuel.UpperLimit += fuelLimit;
    public void UpgradeDurabilityRate(double durabilityRate) => Durability.Rate += durabilityRate;
    public void UpgradeDurabilityLimit(double durabilityLimit) => Durability.UpperLimit += durabilityLimit;
    public void UpgradeSpeedLimit(double speedLimit) => Speed.UpperLimit += speedLimit;
}