using System;
using UnityEngine;
using Newtonsoft.Json;

public class TrainAttribute : Arithmetic, ICloneable
{
    public Attribute<int> Capacity { get; private set; }
    public Attribute<double> Fuel { get; private set; }
    public Attribute<double> Durability { get; private set; }
    public Attribute<double> Speed { get; private set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public TrainDirection Direction { get; private set; }

    [JsonConstructor]
    private TrainAttribute(
        Attribute<int> capacity,
        Attribute<double> fuel,
        Attribute<double> durability,
        Attribute<double> speed,
        Vector3 position,
        Quaternion rotation,
        string direction)
    {
        Capacity = capacity;
        Fuel = fuel;
        Durability = durability;
        Speed = speed;
        Position = position;
        Rotation = rotation;
        Direction = Enum.Parse<TrainDirection>(direction);
    }

    public TrainAttribute(
        Attribute<int> capacity,
        Attribute<double> fuel,
        Attribute<double> durability,
        Attribute<double> speed,
        Vector3 position,
        Quaternion rotation,
        TrainDirection direction)
    {
        Capacity = capacity;
        Fuel = fuel;
        Durability = durability;
        Speed = speed;
        Position = position;
        Rotation = rotation;
        Direction = direction;
    }

    public void SetUnityStats(float speed, Vector3 position, Quaternion rotation, TrainDirection direction)
    {
        Speed.Amount = (double)speed;
        Position = position;
        Rotation = rotation;
        Direction = direction;
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

    public object Clone()
    {
        TrainAttribute attribute = (TrainAttribute)MemberwiseClone();

        // TODO: Deep copy of contents

        return attribute;
    }
}