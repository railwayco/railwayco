using System;
using UnityEngine;
using NUnit.Framework;

public class StationAttributeTests
{
    [Test]
    public void StationAttribute_SetUnityStats_UnityStatsSaved()
    {
        Vector3 vector = new();
        
        StationAttribute stationAttribute = StationAttributeInit();
        stationAttribute.SetUnityStats(vector);

        Assert.AreEqual(vector, stationAttribute.Position);
    }

    [TestCase(50, 50)]
    [TestCase(50, 0)]
    public void StationAttribute_IsYardFull_YardCapacityMaxed(int limit, int amount)
    {
        StationAttribute stationAttribute = StationAttributeInit(yardCapacityLimit: limit, yardCapacityAmount: amount);
        if (amount >= limit)
            Assert.AreEqual(true, stationAttribute.IsYardFull());
        else
            Assert.AreEqual(false, stationAttribute.IsYardFull());
    }

    [TestCase(50)]
    public void StationAttribute_AddToYard_YardCapacityIncreased(int baseValue)
    {
        StationAttribute stationAttribute = StationAttributeInit(yardCapacityAmount: baseValue);
        stationAttribute.AddToYard();
        Assert.AreEqual(stationAttribute.IntAddition(baseValue, 1), stationAttribute.YardCapacity.Amount);
    }

    [TestCase(int.MaxValue)]
    public void StationAttribute_AddToYard_YardCapacityInvalid(int baseValue)
    {
        StationAttribute stationAttribute = StationAttributeInit(yardCapacityAmount: baseValue);
        Assert.Catch<ArithmeticException>(() => stationAttribute.AddToYard());
    }

    [TestCase(1)]
    public void StationAttribute_RemoveFromYard_YardCapacityDecreased(int baseValue)
    {
        StationAttribute stationAttribute = StationAttributeInit(yardCapacityAmount: baseValue);
        stationAttribute.RemoveFromYard();
        Assert.AreEqual(stationAttribute.IntSubtraction(baseValue, 1), stationAttribute.YardCapacity.Amount);
    }

    [TestCase(0)]
    public void StationAttribute_RemoveFromYard_YardCapacityInvalid(int baseValue)
    {
        StationAttribute stationAttribute = StationAttributeInit(yardCapacityAmount: baseValue);
        Assert.Catch<ArithmeticException>(() => stationAttribute.RemoveFromYard());
    }

    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void StationAttribute_UpgradeYardCapacity_YardCapacityIncreased(int baseValue, int increment)
    {
        StationAttribute stationAttribute = StationAttributeInit(yardCapacityLimit: baseValue);
        stationAttribute.UpgradeYardCapacity(increment);
        Assert.AreEqual(stationAttribute.IntAddition(baseValue, increment), stationAttribute.YardCapacity.UpperLimit);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void StationAttribute_UpgradeYardCapacity_YardCapacityInvalid(int baseValue, int increment)
    {
        StationAttribute stationAttribute = StationAttributeInit(yardCapacityLimit: baseValue);
        Assert.Catch<ArgumentException>(() => stationAttribute.UpgradeYardCapacity(increment));
    }

    private StationAttribute StationAttributeInit(
        int yardCapacityLimit = 0,
        int yardCapacityAmount = 0,
        Vector3 position = default)
    {
        StationAttribute stationAttribute = new(
            new(0, yardCapacityLimit, yardCapacityAmount, 0),
            position);
        return stationAttribute;
    }
}
