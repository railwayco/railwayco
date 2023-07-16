using System;
using UnityEngine;
using NUnit.Framework;

public class StationAttributeTests
{
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
        Assert.AreEqual(Arithmetic.IntAddition(baseValue, 1), stationAttribute.YardCapacity.Amount);
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
        Assert.AreEqual(Arithmetic.IntSubtraction(baseValue, 1), stationAttribute.YardCapacity.Amount);
    }

    [TestCase(0)]
    public void StationAttribute_RemoveFromYard_YardCapacityInvalid(int baseValue)
    {
        StationAttribute stationAttribute = StationAttributeInit(yardCapacityAmount: baseValue);
        Assert.Catch<ArithmeticException>(() => stationAttribute.RemoveFromYard());
    }

    [Test]
    public void StationAttribute_Clone_IsDeepCopy()
    {
        StationAttribute stationAttribute = StationAttributeInit(10, 50);
        StationAttribute stationAttributeClone = (StationAttribute)stationAttribute.Clone();
        stationAttributeClone.YardCapacity.Amount = 100;
        Assert.AreNotEqual(stationAttribute, stationAttributeClone);
    }

    private StationAttribute StationAttributeInit(
        int yardCapacityLimit = 0,
        int yardCapacityAmount = 0)
    {
        StationAttribute stationAttribute = new(
            new(0, yardCapacityLimit, yardCapacityAmount, 0));
        return stationAttribute;
    }
}
