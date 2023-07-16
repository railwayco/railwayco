using System;
using System.Collections.Generic;
using NUnit.Framework;

public class CurrencyRangedManagerTests
{
    [TestCase(CurrencyType.Coin, 100, 10)]
    [TestCase(CurrencyType.Note, 100, 99)]
    [TestCase(CurrencyType.NormalCrate, int.MaxValue, 100)]
    public void CurrencyRangedManager_SetCurrencyRanged_CurrencyValueIncreased(
        CurrencyType currencyType,
        int lowerValue,
        int upperValue)
    {
        CurrencyRangedManager currencyRangedManager = CurrencyRangedManagerInit();

        List<Tuple<CurrencyType, int, int>> currencyList = CurrencyListInit(currencyType, lowerValue, upperValue);
        foreach (var currencyTuple in currencyList)
        {
            currencyRangedManager.SetCurrencyRanged(currencyTuple.Item1, currencyTuple.Item2, currencyTuple.Item3);
        }

        foreach (var currencyTuple in currencyList)
        {
            Attribute<int> currency = currencyRangedManager.CurrencyRangedDict[currencyTuple.Item1];
            Assert.AreEqual(currencyTuple.Item2, currency.LowerLimit);
            Assert.AreEqual(currencyTuple.Item3, currency.UpperLimit);
        }
    }

    [Test]
    public void CurrencyRangedManager_Clone_IsDeepCopy()
    {
        CurrencyRangedManager currencyRangedManager = CurrencyRangedManagerInitPopulated(100, 200, 300, 400);
        CurrencyRangedManager cloneCurrencyRangedManager = (CurrencyRangedManager)currencyRangedManager.Clone();
        cloneCurrencyRangedManager.Randomise();

        foreach(CurrencyType currencyType in cloneCurrencyRangedManager.CurrencyRangedDict.Keys)
        {
            Assert.AreNotEqual(
                currencyRangedManager.CurrencyRangedDict[currencyType], 
                cloneCurrencyRangedManager.CurrencyRangedDict[currencyType],
                currencyType.ToString());
        }
    }

    private CurrencyRangedManager CurrencyRangedManagerInit() => new();

    private List<Tuple<CurrencyType, int, int>> CurrencyListInit(CurrencyType currencyType, int lowerValue, int upperValue)
    {
        List<Tuple<CurrencyType, int, int>> currencyList = new();

        for (int i = 0; i < 3; i++)
        {
            Tuple<CurrencyType, int, int> currency = new(currencyType, lowerValue, upperValue);
            currencyList.Add(currency);
        }

        return currencyList;
    }

    private CurrencyRangedManager CurrencyRangedManagerInitPopulated(
        int coinValue,
        int noteValue,
        int normalCrateValue,
        int specialCrateValue)
    {
        CurrencyRangedManager currencyRangedManager = new();

        List<Tuple<CurrencyType, int, int>> coinList = CurrencyListInit(CurrencyType.Coin, coinValue, coinValue + 50);
        List<Tuple<CurrencyType, int, int>> noteList = CurrencyListInit(CurrencyType.Note, noteValue, noteValue + 50);
        List<Tuple<CurrencyType, int, int>> normalCrateList = CurrencyListInit(CurrencyType.NormalCrate, normalCrateValue, normalCrateValue + 50);
        List<Tuple<CurrencyType, int, int>> specialCrateList = CurrencyListInit(CurrencyType.SpecialCrate, specialCrateValue, specialCrateValue + 50);

        List<List<Tuple<CurrencyType, int, int>>> currencies = new()
        {
            coinList,
            noteList,
            normalCrateList,
            specialCrateList
        };

        foreach (var currencyList in currencies)
        {
            foreach (var currencyTuple in currencyList)
            {
                currencyRangedManager.SetCurrencyRanged(currencyTuple.Item1, currencyTuple.Item2, currencyTuple.Item3);
            }
        }

        return currencyRangedManager;
    }
}
