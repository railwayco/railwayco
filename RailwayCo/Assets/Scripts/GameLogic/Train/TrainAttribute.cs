using System;
using UnityEngine;
using Newtonsoft.Json;

public class TrainAttribute : ICloneable, IEquatable<TrainAttribute>
{
    public IntAttribute Capacity { get; private set; }
    public DoubleAttribute Fuel { get; private set; }
    public DoubleAttribute Durability { get; private set; }
    public DoubleAttribute Speed { get; private set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public MovementDirection MovementDirection { get; private set; }
    public MovementState MovementState { get; private set; }


    [JsonConstructor]
    private TrainAttribute(
        IntAttribute capacity,
        DoubleAttribute fuel,
        DoubleAttribute durability,
        DoubleAttribute speed,
        Vector3 position,
        Quaternion rotation,
        string movementDirection,
        string movementState)
    {
        Capacity = capacity;
        Fuel = fuel;
        Durability = durability;
        Speed = speed;
        Position = position;
        Rotation = rotation;
        MovementDirection = Enum.Parse<MovementDirection>(movementDirection);
        MovementState = Enum.Parse<MovementState>(movementState);
    }

    public TrainAttribute(
        IntAttribute capacity,
        DoubleAttribute fuel,
        DoubleAttribute durability,
        DoubleAttribute speed,
        Vector3 position,
        Quaternion rotation,
        MovementDirection movementDirection,
        MovementState movementState)
    {
        Capacity = capacity;
        Fuel = fuel;
        Durability = durability;
        Speed = speed;
        Position = position;
        Rotation = rotation;
        MovementDirection = movementDirection;
        MovementState = movementState;
    }

    public void SetUnityStats(float speed,
                              Vector3 position,
                              Quaternion rotation,
                              MovementDirection movementDirection,
                              MovementState movementState)
    {
        Speed.Amount = speed;
        Position = position;
        Rotation = rotation;
        MovementDirection = movementDirection;
        MovementState = movementState;
    }

    public bool IsCapacityFull() => Capacity.Amount >= Capacity.UpperLimit;
    public void AddToCapacity()
    {
        if (Capacity.Amount == int.MaxValue) throw new ArithmeticException("Capacity cannot go above limit of int");
        Capacity.Amount = Arithmetic.IntAddition(Capacity.Amount, 1);
    }
    public void RemoveFromCapacity()
    {
        if (Capacity.Amount == 0) throw new ArithmeticException("Capacity cannot go below zero");
        Capacity.Amount = Arithmetic.IntSubtraction(Capacity.Amount, 1);
    }

    public bool BurnFuel(int fuelToBurn)
    {
        double newAmount = Arithmetic.DoubleRangeCheck(Fuel.Amount - fuelToBurn);
        if (newAmount < Fuel.LowerLimit) return false;
        Fuel.Amount = newAmount;
        return true;
    }

    public bool Refuel()
    {
        if (Fuel.Amount == Fuel.UpperLimit) return false;
        Fuel.Amount = Arithmetic.DoubleRangeCheck(Fuel.Amount + Fuel.Rate);
        if (Fuel.Amount > Fuel.LowerLimit) Fuel.Amount = Fuel.UpperLimit;
        return true;
    }

    public bool DurabilityWear()
    {
        double newAmount = Arithmetic.DoubleRangeCheck(Durability.Amount - Durability.Rate);
        if (newAmount < Durability.LowerLimit) return false;
        Durability.Amount = newAmount;
        return true;
    }

    public bool DurabilityRepair()
    {
        if (Durability.Amount == Durability.UpperLimit) return false;
        Durability.Amount = Arithmetic.DoubleRangeCheck(Durability.Amount + Durability.Rate);
        if (Durability.Amount > Durability.LowerLimit) Durability.Amount = Durability.UpperLimit;
        return true;
    }

    public object Clone()
    {
        TrainAttribute attribute = (TrainAttribute)MemberwiseClone();

        attribute.Capacity = (IntAttribute)Capacity.Clone();
        attribute.Fuel = (DoubleAttribute)Fuel.Clone();
        attribute.Durability = (DoubleAttribute)Durability.Clone();
        attribute.Speed = (DoubleAttribute)Speed.Clone();
        attribute.Position = new(Position.x, Position.y, Position.z);
        attribute.Rotation = new(Rotation.x, Rotation.y, Rotation.z, Rotation.w);

        return attribute;
    }

    public bool Equals(TrainAttribute other)
    {
        return Capacity.Equals(other.Capacity)
            && Fuel.Equals(other.Fuel)
            && Durability.Equals(other.Durability)
            && Speed.Equals(other.Speed)
            && Position.Equals(other.Position)
            && Rotation.Equals(other.Rotation)
            && MovementDirection.Equals(other.MovementDirection)
            && MovementState.Equals(other.MovementState);
    }
}