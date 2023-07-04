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
        Speed.Amount = speed;
        Position = position;
        Rotation = rotation;
        Direction = direction;
    }

    public bool IsCapacityFull() => Capacity.Amount >= Capacity.UpperLimit;
    public void AddToCapacity()
    {
        if (Capacity.Amount == int.MaxValue) throw new ArithmeticException("Capacity cannot go above limit of int");
        Capacity.Amount = IntAddition(Capacity.Amount, 1);
    }
    public void RemoveFromCapacity()
    {
        if (Capacity.Amount == 0) throw new ArithmeticException("Capacity cannot go below zero");
        Capacity.Amount = IntSubtraction(Capacity.Amount, 1);
    }

    public bool BurnFuel()
    {
        double newAmount = DoubleRangeCheck(Fuel.Amount - Fuel.Rate);
        if (newAmount < Fuel.LowerLimit) return false;
        Fuel.Amount = newAmount;
        return true;
    }

    public bool Refuel()
    {
        if (Fuel.Amount == Fuel.UpperLimit) return false;
        Fuel.Amount = DoubleRangeCheck(Fuel.Amount + Fuel.Rate);
        if (Fuel.Amount > Fuel.LowerLimit) Fuel.Amount = Fuel.UpperLimit;
        return true;
    }

    public bool DurabilityWear()
    {
        double newAmount = DoubleRangeCheck(Durability.Amount - Durability.Rate);
        if (newAmount < Durability.LowerLimit) return false;
        Durability.Amount = newAmount;
        return true;
    }

    public bool DurabilityRepair()
    {
        if (Durability.Amount == Durability.UpperLimit) return false;
        Durability.Amount = DoubleRangeCheck(Durability.Amount + Durability.Rate);
        if (Durability.Amount > Durability.LowerLimit) Durability.Amount = Durability.UpperLimit;
        return true;
    }

    public object Clone()
    {
        TrainAttribute attribute = (TrainAttribute)MemberwiseClone();

        // TODO: Deep copy of contents

        return attribute;
    }
}