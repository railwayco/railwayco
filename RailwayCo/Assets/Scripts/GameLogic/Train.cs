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
    private CargoManager cargoManager;

    public string TrainName { get => trainName; set => trainName = value; }
    public TrainType TrainType { get => trainType; private set => trainType = value; }
    public Attribute<int> TrainCapacity { get => trainCapacity; private set => trainCapacity = value; }
    public Attribute<float> TrainFuel { get => trainFuel; private set => trainFuel = value; }
    public Attribute<float> TrainDurability { get => trainDurability; private set => trainDurability = value; }
    public Attribute<float> TrainSpeed { get => trainSpeed; private set => trainSpeed = value; }
    public CargoManager CargoManager { get => cargoManager; private set => cargoManager = value; }

    public Train(
        string trainName,
        TrainType trainType,
        Attribute<int> trainCapacity,
        Attribute<float> trainFuel,
        Attribute<float> trainDurability,
        Attribute<float> trainSpeed,
        CargoManager cargoManager)
    {
        TrainName = trainName;
        TrainType = trainType;
        TrainCapacity = trainCapacity;
        TrainFuel = trainFuel;
        TrainDurability = trainDurability;
        TrainSpeed = trainSpeed;
        CargoManager = cargoManager;
    }

    public Dictionary<string, string> UpgradeCapacityLimit(int capacityLimit) 
    {
        Dictionary<string, string> result = new()
        {
            { "old", TrainCapacity.UpperLimit.ToString() }
        };

        TrainCapacity.UpperLimit += capacityLimit;
        result.Add("new", TrainCapacity.UpperLimit.ToString());

        return result;
    }

    public Dictionary<string, string> UpgradeFuelRate(float fuelRate)
    {
        Dictionary<string, string> result = new()
        {
            { "old", TrainFuel.Rate.ToString() }
        };

        TrainFuel.Rate += fuelRate;
        result.Add("new", TrainFuel.Rate.ToString());

        return result;
    }

    public Dictionary<string, string> UpgradeFuelLimit(float fuelLimit)
    {
        Dictionary<string, string> result = new()
        {
            { "old", TrainFuel.UpperLimit.ToString() }
        };

        TrainFuel.UpperLimit += fuelLimit;
        result.Add("new", TrainFuel.UpperLimit.ToString());

        return result;
    }

    public Dictionary<string, string> UpgradeDurabilityRate(float durabilityRate)
    {
        Dictionary<string, string> result = new()
        {
            { "old", TrainDurability.Rate.ToString() }
        };

        TrainDurability.Rate += durabilityRate;
        result.Add("new", TrainDurability.Rate.ToString());

        return result;
    }

    public Dictionary<string, string> UpgradeDurabilityLimit(float durabilityLimit)
    {
        Dictionary<string, string> result = new()
        {
            { "old", TrainDurability.UpperLimit.ToString() }
        };

        TrainDurability.UpperLimit += durabilityLimit;
        result.Add("new", TrainDurability.UpperLimit.ToString());

        return result;
    }

    public Dictionary<string, string> UpgradeSpeedLimit(float speedLimit)
    {
        Dictionary<string, string> result = new()
        {
            { "old", TrainSpeed.UpperLimit.ToString() }
        };

        TrainSpeed.UpperLimit += speedLimit;
        result.Add("new", TrainSpeed.UpperLimit.ToString());

        return result;
    }
}

public enum TrainType
{
    Steam,
    Diesel,
    Electric
}

public class TrainManager
{
    private Dictionary<string, Train> trainDict;

    private Dictionary<string, Train> TrainDict { get => trainDict; set => trainDict = value; }

    public TrainManager()
    {
        TrainDict = new();
    }

    public void AddTrain(Train train)
    {
        TrainDict.Add(train.TrainName, train);
    }

    public bool RemoveTrain(Train train)
    {
        return TrainDict.Remove(train.TrainName);
    }

    public List<string> GetTrainList()
    {
        return new List<string>(TrainDict.Keys);
    }
}
