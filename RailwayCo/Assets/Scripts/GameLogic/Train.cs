using System.Collections;
using System.Collections.Generic;

public class Train
{
    private string trainName;
    private TrainType trainType;
    private Attribute<int> trainCapacity;
    private Attribute<float> trainFuel;
    private Attribute<float> trainDurability;
    private Attribute<float> trainSpeed;

    public string TrainName { get => trainName; set => trainName = value; }
    public TrainType TrainType { get => trainType; private set => trainType = value; }
    public Attribute<int> TrainCapacity { get => trainCapacity; private set => trainCapacity = value; }
    public Attribute<float> TrainFuel { get => trainFuel; private set => trainFuel = value; }
    public Attribute<float> TrainDurability { get => trainDurability; private set => trainDurability = value; }
    public Attribute<float> TrainSpeed { get => trainSpeed; private set => trainSpeed = value; }

    public Train(
        string trainName,
        TrainType trainType,
        Attribute<int> trainCapacity,
        Attribute<float> trainFuel,
        Attribute<float> trainDurability,
        Attribute<float> trainSpeed)
    {
        TrainName = trainName;
        TrainCapacity = trainCapacity;
        TrainFuel = trainFuel;
        TrainDurability = trainDurability;
        TrainSpeed = trainSpeed;
        TrainType = trainType;
    }

    public Dictionary<string, string> UpgradeCapacityLimit(int capacityLimit) 
    {
        Dictionary<string, string> result = new()
        {
            { "old", trainCapacity.UpperLimit.ToString() }
        };

        trainCapacity.UpperLimit += capacityLimit;
        result.Add("new", trainCapacity.UpperLimit.ToString());

        return result;
    }

    public Dictionary<string, string> UpgradeFuelRate(float fuelRate)
    {
        Dictionary<string, string> result = new()
        {
            { "old", trainFuel.Rate.ToString() }
        };

        trainFuel.Rate += fuelRate;
        result.Add("new", trainFuel.Rate.ToString());

        return result;
    }

    public Dictionary<string, string> UpgradeFuelLimit(float fuelLimit)
    {
        Dictionary<string, string> result = new()
        {
            { "old", trainFuel.UpperLimit.ToString() }
        };

        trainFuel.UpperLimit += fuelLimit;
        result.Add("new", trainFuel.UpperLimit.ToString());

        return result;
    }

    public Dictionary<string, string> UpgradeDurabilityRate(float durabilityRate)
    {
        Dictionary<string, string> result = new()
        {
            { "old", trainDurability.Rate.ToString() }
        };

        trainDurability.Rate += durabilityRate;
        result.Add("new", trainDurability.Rate.ToString());

        return result;
    }

    public Dictionary<string, string> UpgradeDurabilityLimit(float durabilityLimit)
    {
        Dictionary<string, string> result = new()
        {
            { "old", trainDurability.UpperLimit.ToString() }
        };

        trainDurability.UpperLimit += durabilityLimit;
        result.Add("new", trainDurability.UpperLimit.ToString());

        return result;
    }

    public Dictionary<string, string> UpgradeSpeedLimit(float speedLimit)
    {
        Dictionary<string, string> result = new()
        {
            { "old", trainSpeed.UpperLimit.ToString() }
        };

        trainSpeed.UpperLimit += speedLimit;
        result.Add("new", trainSpeed.UpperLimit.ToString());

        return result;
    }
}

public enum TrainType
{
    Steam,
    Diesel,
    Electric
}
