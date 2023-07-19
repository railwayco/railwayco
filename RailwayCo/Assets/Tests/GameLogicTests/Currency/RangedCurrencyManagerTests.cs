using System;
using System.Collections.Generic;
using NUnit.Framework;

public class RangedCurrencyManagerTests
{
    [TestCase(CurrencyType.Coin, 100, 10)]
    [TestCase(CurrencyType.Note, 100, 99)]
    [TestCase(CurrencyType.NormalCrate, int.MaxValue, 100)]
    public void RangedCurrencyManager_SetCurrencyRanged_CurrencyValueIncreased(
        CurrencyType currencyType,
        int lowerValue,
        int upperValue)
    {
        RangedCurrencyManager rangedCurrencyManager = RangedCurrencyManagerInit();

        List<Tuple<CurrencyType, int, int>> currencyList = CurrencyListInit(currencyType, lowerValue, upperValue);
        foreach (var currencyTuple in currencyList)
        {
            rangedCurrencyManager.SetRangedCurrency(currencyTuple.Item1, currencyTuple.Item2, currencyTuple.Item3);
        }

        foreach (var currencyTuple in currencyList)
        {
            Attribute<int> currency = rangedCurrencyManager.GetRangedCurrency(currencyTuple.Item1);
            Assert.AreEqual(currencyTuple.Item2, currency.LowerLimit);
            Assert.AreEqual(currencyTuple.Item3, currency.UpperLimit);
        }
    }

    [Test]
    public void RangedCurrencyManager_Randomise_IsCurrencyAmountSet()
    {
        RangedCurrencyManager rangedCurrencyManager = RangedCurrencyManagerInitPopulated(100, 200, 300, 400);
        rangedCurrencyManager.Randomise();

        foreach (CurrencyType currencyType in rangedCurrencyManager.CurrencyRangedDict.Keys)
        {
            Assert.AreNotEqual(0, rangedCurrencyManager.GetRangedCurrency(currencyType), currencyType.ToString());
        }
    }

    [Test]
    public void RangedCurrencyManager_InitCurrencyManager_CorrectValuesSet()
    {
        RangedCurrencyManager rangedCurrencyManager = RangedCurrencyManagerInitPopulated(100, 200, 300, 400);
        rangedCurrencyManager.Randomise();
        CurrencyManager currencyManager = rangedCurrencyManager.InitCurrencyManager();

        foreach (CurrencyType currencyType in rangedCurrencyManager.CurrencyRangedDict.Keys)
        {
            Assert.AreEqual(
                rangedCurrencyManager.GetRangedCurrency(currencyType).Amount,
                currencyManager.GetCurrency(currencyType),
                currencyType.ToString());
        }
    }

    [Test]
    public void RangedCurrencyManager_Clone_IsDeepCopy()
    {
        RangedCurrencyManager rangedCurrencyManager = RangedCurrencyManagerInitPopulated(100, 200, 300, 400);
        RangedCurrencyManager cloneRangedCurrencyManager = (RangedCurrencyManager)rangedCurrencyManager.Clone();
        cloneRangedCurrencyManager.Randomise();

        foreach(CurrencyType currencyType in cloneRangedCurrencyManager.CurrencyRangedDict.Keys)
        {
            Assert.AreNotEqual(
                rangedCurrencyManager.GetRangedCurrency(currencyType), 
                cloneRangedCurrencyManager.GetRangedCurrency(currencyType),
                currencyType.ToString());
        }
    }

    private RangedCurrencyManager RangedCurrencyManagerInit() => new();

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

    private RangedCurrencyManager RangedCurrencyManagerInitPopulated(
        int coinValue,
        int noteValue,
        int normalCrateValue,
        int specialCrateValue)
    {
        RangedCurrencyManager rangedCurrencyManager = new();

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
                rangedCurrencyManager.SetRangedCurrency(currencyTuple.Item1, currencyTuple.Item2, currencyTuple.Item3);
            }
        }

        return rangedCurrencyManager;
    }
}
