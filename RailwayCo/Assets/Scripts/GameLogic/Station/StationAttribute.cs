using System;
using UnityEngine;
using Newtonsoft.Json;

public class StationAttribute : ICloneable, IEquatable<StationAttribute>
{
    public Attribute<int> YardCapacity { get; private set; }

    [JsonConstructor]
    public StationAttribute(Attribute<int> yardCapacity)
    {
        YardCapacity = yardCapacity;
    }

    public bool IsYardFull() => YardCapacity.Amount >= YardCapacity.UpperLimit;
    public void AddToYard()
    {
        if (YardCapacity.Amount == int.MaxValue) throw new ArithmeticException("Yard Capacity cannot go above limit of int");
        YardCapacity.Amount = Arithmetic.IntAddition(YardCapacity.Amount, 1);
    }
    public void RemoveFromYard()
    {
        if (YardCapacity.Amount == 0) throw new ArithmeticException("Yard Capacity cannot go below zero");
        YardCapacity.Amount = Arithmetic.IntSubtraction(YardCapacity.Amount, 1);
    }

    public object Clone()
    {
        StationAttribute attribute = (StationAttribute)MemberwiseClone();

        attribute.YardCapacity = (Attribute<int>)YardCapacity.Clone();

        return attribute;
    }

    public bool Equals(StationAttribute other)
    {
        return YardCapacity.Equals(other.YardCapacity);
    }
}
