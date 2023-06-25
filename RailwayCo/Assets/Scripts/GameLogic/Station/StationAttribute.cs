using Newtonsoft.Json;
using System;
using UnityEngine;

public class StationAttribute : Arithmetic, ICloneable
{
    public Attribute<int> YardCapacity { get; private set; }
    public Vector3 Position { get; private set; }

    [JsonConstructor]
    public StationAttribute(Attribute<int> yardCapacity, Vector3 position)
    {
        YardCapacity = yardCapacity;
        Position = position;
    }

    public void SetUnityStats(Vector3 position) => Position = position;

    public bool IsYardFull() => YardCapacity.Amount == YardCapacity.UpperLimit;
    public void AddToYard() => YardCapacity.Amount = IntAddition(YardCapacity.Amount, 1);
    public void RemoveFromYard() => YardCapacity.Amount = IntSubtraction(YardCapacity.Amount, 1);
    public void UpgradeYardCapacity(int yardCapacity)
    {
        if (yardCapacity < 0.0) throw new System.ArgumentException("Invalid yard capacity");
        YardCapacity.UpperLimit = IntAddition(YardCapacity.Amount, yardCapacity);
    }

    public object Clone()
    {
        StationAttribute attribute = (StationAttribute)MemberwiseClone();

        // TODO: Deep copy of contents

        return attribute;
    }
}
