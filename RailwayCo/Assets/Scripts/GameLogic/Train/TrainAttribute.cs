public class TrainAttribute
{
    private Attribute<int> trainCapacity;
    private Attribute<double> trainFuel;
    private Attribute<double> trainDurability;
    private Attribute<double> trainSpeed;

    public Attribute<int> TrainCapacity { get => trainCapacity; private set => trainCapacity = value; }
    public Attribute<double> TrainFuel { get => trainFuel; private set => trainFuel = value; }
    public Attribute<double> TrainDurability { get => trainDurability; private set => trainDurability = value; }
    public Attribute<double> TrainSpeed { get => trainSpeed; private set => trainSpeed = value; }

    public void UpgradeCapacityLimit(int capacityLimit) => TrainCapacity.UpperLimit += capacityLimit;
    public void UpgradeFuelRate(double fuelRate) => TrainFuel.Rate += fuelRate;
    public void UpgradeFuelLimit(double fuelLimit) => TrainFuel.UpperLimit += fuelLimit;
    public void UpgradeDurabilityRate(double durabilityRate) => TrainDurability.Rate += durabilityRate;
    public void UpgradeDurabilityLimit(double durabilityLimit) => TrainDurability.UpperLimit += durabilityLimit;
    public void UpgradeSpeedLimit(double speedLimit) => TrainSpeed.UpperLimit += speedLimit;
}