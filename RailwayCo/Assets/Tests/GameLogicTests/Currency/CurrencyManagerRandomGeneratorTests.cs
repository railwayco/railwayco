using System;
using System.Collections.Generic;
using NUnit.Framework;

public class CurrencyManagerRandomGeneratorTests
{
    [TestCase(CurrencyType.Coin, 100, 10)]
    [TestCase(CurrencyType.Note, 100, 99)]
    [TestCase(CurrencyType.NormalCrate, int.MaxValue, 100)]
    public void CurrencyManagerRandomGenerator_SetCurrencyRanged_CurrencyValueIncreased(
        CurrencyType currencyType,
        int lowerValue,
        int upperValue)
    {
        CurrencyManagerRandomGenerator currencyManager = CurrencyManagerRandomGeneratorInit();

        List<Tuple<CurrencyType, int, int>> currencyList = CurrencyListInit(currencyType, lowerValue, upperValue);
        foreach (var currencyTuple in currencyList)
        {
            currencyManager.SetCurrencyRanged(currencyTuple.Item1, currencyTuple.Item2, currencyTuple.Item3);
        }

        foreach (var currencyTuple in currencyList)
        {
            Attribute<int> currency = currencyManager.CurrencyRangedDict[currencyTuple.Item1];
            Assert.AreEqual(currencyTuple.Item2, currency.LowerLimit);
            Assert.AreEqual(currencyTuple.Item3, currency.UpperLimit);
        }
    }

    [Test]
    public void CurrencyManagerRandomGenerator_Randomise_IsCurrencyAmountSet()
    {
        CurrencyManagerRandomGenerator currencyManager = CurrencyManagerRandomGeneratorInitPopulated(100, 200, 300, 400);
        currencyManager.Randomise();

        foreach (CurrencyType currencyType in currencyManager.CurrencyRangedDict.Keys)
        {
            Assert.AreNotEqual(0, currencyManager.CurrencyRangedDict[currencyType], currencyType.ToString());
        }
    }

    [Test]
    public void CurrencyManagerRandomGenerator_InitCurrencyManager_CorrectValuesSet()
    {
        CurrencyManagerRandomGenerator currencyManagerRG = CurrencyManagerRandomGeneratorInitPopulated(100, 200, 300, 400);
        currencyManagerRG.Randomise();
        CurrencyManager currencyManager = currencyManagerRG.InitCurrencyManager();

        foreach (CurrencyType currencyType in currencyManagerRG.CurrencyRangedDict.Keys)
        {
            Assert.AreEqual(
                currencyManagerRG.CurrencyRangedDict[currencyType].Amount,
                currencyManager.GetCurrency(currencyType),
                currencyType.ToString());
        }
    }

    [Test]
    public void CurrencyManagerRandomGenerator_Clone_IsDeepCopy()
    {
        CurrencyManagerRandomGenerator currencyManager = CurrencyManagerRandomGeneratorInitPopulated(100, 200, 300, 400);
        CurrencyManagerRandomGenerator cloneCurrencyManagerRandomGenerator = (CurrencyManagerRandomGenerator)currencyManager.Clone();
        cloneCurrencyManagerRandomGenerator.Randomise();

        foreach(CurrencyType currencyType in cloneCurrencyManagerRandomGenerator.CurrencyRangedDict.Keys)
        {
            Assert.AreNotEqual(
                currencyManager.CurrencyRangedDict[currencyType], 
                cloneCurrencyManagerRandomGenerator.CurrencyRangedDict[currencyType],
                currencyType.ToString());
        }
    }

    private CurrencyManagerRandomGenerator CurrencyManagerRandomGeneratorInit() => new();

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

    private CurrencyManagerRandomGenerator CurrencyManagerRandomGeneratorInitPopulated(
        int coinValue,
        int noteValue,
        int normalCrateValue,
        int specialCrateValue)
    {
        CurrencyManagerRandomGenerator currencyManager = new();

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
                currencyManager.SetCurrencyRanged(currencyTuple.Item1, currencyTuple.Item2, currencyTuple.Item3);
            }
        }

        return currencyManager;
    }
}
