public class TrainAttribute
{
    private Attribute<int> trainCapacity;
    private Attribute<float> trainFuel;
    private Attribute<float> trainDurability;
    private Attribute<float> trainSpeed;

    public Attribute<int> TrainCapacity { get => trainCapacity; private set => trainCapacity = value; }
    public Attribute<float> TrainFuel { get => trainFuel; private set => trainFuel = value; }
    public Attribute<float> TrainDurability { get => trainDurability; private set => trainDurability = value; }
    public Attribute<float> TrainSpeed { get => trainSpeed; private set => trainSpeed = value; }

    public void UpgradeCapacityLimit(int capacityLimit) => TrainCapacity.UpperLimit += capacityLimit;
    public void UpgradeFuelRate(float fuelRate) => TrainFuel.Rate += fuelRate;
    public void UpgradeFuelLimit(float fuelLimit) => TrainFuel.UpperLimit += fuelLimit;
    public void UpgradeDurabilityRate(float durabilityRate) => TrainDurability.Rate += durabilityRate;
    public void UpgradeDurabilityLimit(float durabilityLimit) => TrainDurability.UpperLimit += durabilityLimit;
    public void UpgradeSpeedLimit(float speedLimit) => TrainSpeed.UpperLimit += speedLimit;
}