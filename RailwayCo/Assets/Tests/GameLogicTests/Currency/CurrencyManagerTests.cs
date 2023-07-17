using System;
using System.Collections.Generic;
using NUnit.Framework;

public class CurrencyManagerTests
{
    [TestCase(CurrencyType.Coin, 100, 10)]
    [TestCase(CurrencyType.Note, 100, 99)]
    [TestCase(CurrencyType.NormalCrate, int.MaxValue, 100)]
    public void CurrencyManager_AddCurrency_CurrencyValueIncreased(
        CurrencyType currencyType,
        int baseValue,
        int increment)
    {
        CurrencyManager currencyManager = CurrencyManagerInit();

        List<Tuple<CurrencyType, int>> currencyList = CurrencyListInit(currencyType, baseValue);
        int expected = 0;
        foreach (var currencyTuple in currencyList)
        {
            currencyManager.AddCurrency(currencyTuple.Item1, currencyTuple.Item2);
            expected = Arithmetic.IntAddition(expected, currencyTuple.Item2);
        }

        List<Tuple<CurrencyType, int>> incrementCurrencyList = CurrencyListInit(currencyType, increment);
        foreach (var currencyTuple in incrementCurrencyList)
        {
            currencyManager.AddCurrency(currencyTuple.Item1, currencyTuple.Item2);
            expected = Arithmetic.IntAddition(expected, currencyTuple.Item2);
        }

        int actual = currencyManager.CurrencyDict[currencyType];
        Assert.AreEqual(expected, actual);
    }

    [TestCase(CurrencyType.Coin, 100, 10)]
    [TestCase(CurrencyType.Note, 100, 99)]
    [TestCase(CurrencyType.NormalCrate, int.MaxValue, 100)]
    public void CurrencyManager_RemoveCurrency_CurrencyValueDecreased(
        CurrencyType currencyType,
        int baseValue,
        int increment)
    {
        CurrencyManager currencyManager = CurrencyManagerInit();

        List<Tuple<CurrencyType, int>> baseCurrencyList = CurrencyListInit(currencyType, baseValue);
        int expected = 0;
        foreach (var currencyTuple in baseCurrencyList)
        {
            currencyManager.AddCurrency(currencyTuple.Item1, currencyTuple.Item2);
            expected = Arithmetic.IntAddition(expected, currencyTuple.Item2);
        }

        List<Tuple<CurrencyType, int>> incrementCurrencyList = CurrencyListInit(currencyType, increment);
        foreach (var currencyTuple in incrementCurrencyList)
        {
            currencyManager.RemoveCurrency(currencyTuple.Item1, currencyTuple.Item2);
            expected = Arithmetic.IntSubtraction(expected, currencyTuple.Item2);
        }

        int actual = currencyManager.CurrencyDict[currencyType];
        Assert.AreEqual(expected, actual);
    }

    [TestCase(100, 10, 200, 10)]
    [TestCase(100, 99, 2000, 100)]
    [TestCase(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue)]
    public void CurrencyManager_AddCurrencyManager_CurrencyValueIncreased(
        int coinValue,
        int noteValue,
        int normalCrateValue,
        int specialCrateValue)
    {
        CurrencyManager currencyManager = CurrencyManagerInit();
        CurrencyManager baseCurrencyManager = CurrencyManagerInitPopulated(2000, 103, 200, 10);
        CurrencyManager incrementCurrencyManager = CurrencyManagerInitPopulated(
            coinValue,
            noteValue,
            normalCrateValue,
            specialCrateValue);
        currencyManager.AddCurrencyManager(baseCurrencyManager);
        currencyManager.AddCurrencyManager(incrementCurrencyManager);

        List<CurrencyType> currencyTypes = new(baseCurrencyManager.CurrencyDict.Keys);
        foreach (CurrencyType currencyType in currencyTypes)
        {
            int expected = baseCurrencyManager.CurrencyDict[currencyType];
            expected = Arithmetic.IntAddition(expected, incrementCurrencyManager.CurrencyDict[currencyType]);
            Assert.AreEqual(expected, currencyManager.CurrencyDict[currencyType]);
        }
    }

    [TestCase(100, 10, 200, 10)]
    [TestCase(100, 99, 2000, 100)]
    [TestCase(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue)]
    public void CurrencyManager_RemoveCurrencyManager_CurrencyValueDecreased(
        int coinValue,
        int noteValue,
        int normalCrateValue,
        int specialCrateValue)
    {
        CurrencyManager currencyManager = CurrencyManagerInit();
        CurrencyManager baseCurrencyManager = CurrencyManagerInitPopulated(2000, 103, 200, 10);
        CurrencyManager incrementCurrencyManager = CurrencyManagerInitPopulated(
            coinValue,
            noteValue,
            normalCrateValue,
            specialCrateValue);
        currencyManager.AddCurrencyManager(baseCurrencyManager);
        currencyManager.RemoveCurrencyManager(incrementCurrencyManager);

        List<CurrencyType> currencyTypes = new(baseCurrencyManager.CurrencyDict.Keys);
        foreach (CurrencyType currencyType in currencyTypes)
        {
            int expected = baseCurrencyManager.CurrencyDict[currencyType];
            expected = Arithmetic.IntSubtraction(expected, incrementCurrencyManager.CurrencyDict[currencyType]);
            Assert.AreEqual(expected, currencyManager.CurrencyDict[currencyType]);
        }
    }

    [Test]
    public void CurrencyManager_Clone_IsDeepCopy()
    {
        CurrencyManager currencyManager = CurrencyManagerInitPopulated(100, 100, 100, 100);
        CurrencyManager cloneCurrencyManager = (CurrencyManager)currencyManager.Clone();
        cloneCurrencyManager.AddCurrencyManager(currencyManager);

        foreach(CurrencyType currencyType in cloneCurrencyManager.CurrencyDict.Keys)
        {
            Assert.AreNotEqual(
                currencyManager.CurrencyDict[currencyType], 
                cloneCurrencyManager.CurrencyDict[currencyType],
                currencyType.ToString());
        }
    }

    private CurrencyManager CurrencyManagerInit() => new();

    private List<Tuple<CurrencyType, int>> CurrencyListInit(CurrencyType currencyType, int baseValue)
    {
        List<Tuple<CurrencyType, int>> currencyList = new();

        for (int i = 0; i < 3; i++)
        {
            Tuple<CurrencyType, int> currency = new(currencyType, baseValue);
            currencyList.Add(currency);
        }

        return currencyList;
    }

    private CurrencyManager CurrencyManagerInitPopulated(
        int coinValue,
        int noteValue,
        int normalCrateValue,
        int specialCrateValue)
    {
        CurrencyManager currencyManager = new();

        List<Tuple<CurrencyType, int>> coinList = CurrencyListInit(CurrencyType.Coin, coinValue);
        List<Tuple<CurrencyType, int>> noteList = CurrencyListInit(CurrencyType.Note, noteValue);
        List<Tuple<CurrencyType, int>> normalCrateList = CurrencyListInit(CurrencyType.NormalCrate, normalCrateValue);
        List<Tuple<CurrencyType, int>> specialCrateList = CurrencyListInit(CurrencyType.SpecialCrate, specialCrateValue);

        List<List<Tuple<CurrencyType, int>>> currencies = new()
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
                currencyManager.AddCurrency(currencyTuple.Item1, currencyTuple.Item2);
            }
        }

        return currencyManager;
    }
}
