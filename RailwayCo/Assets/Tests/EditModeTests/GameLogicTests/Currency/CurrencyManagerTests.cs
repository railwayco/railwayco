using System;
using System.Collections.Generic;
using NUnit.Framework;

public class CurrencyManagerTests
{
    [TestCase(CurrencyType.Coin, 100.0, 10)]
    [TestCase(CurrencyType.Note, 100.5, 99.9)]
    [TestCase(CurrencyType.NormalCrate, double.MaxValue, 100.3)]
    public void CurrencyManager_AddCurrency_CurrencyValueIncreased(
        CurrencyType currencyType,
        double baseValue,
        double increment)
    {
        CurrencyManager currencyManager = CurrencyManagerInit();

        List<Currency> currencyList = CurrencyListInit(currencyType, baseValue);
        double expected = 0.0;
        foreach (var currency in currencyList)
        {
            currencyManager.AddCurrency(currency);
            expected = Arithmetic.DoubleRangeCheck(expected + currency.CurrencyValue);
        }

        List<Currency> incrementCurrencyList = CurrencyListInit(currencyType, increment);
        foreach (var currency in incrementCurrencyList)
        {
            currencyManager.AddCurrency(currency);
            expected = Arithmetic.DoubleRangeCheck(expected + currency.CurrencyValue);
        }

        double actual = currencyManager.CurrencyDict[currencyType].CurrencyValue;
        Assert.AreEqual(expected, actual);
    }

    [TestCase(CurrencyType.Coin, 100.0, 10)]
    [TestCase(CurrencyType.Note, 100.5, 99.9)]
    [TestCase(CurrencyType.NormalCrate, double.MaxValue, 100.3)]
    public void CurrencyManager_RemoveCurrency_CurrencyValueDecreased(
        CurrencyType currencyType,
        double baseValue,
        double increment)
    {
        CurrencyManager currencyManager = CurrencyManagerInit();

        List<Currency> baseCurrencyList = CurrencyListInit(currencyType, baseValue);
        double expected = 0.0;
        foreach (var currency in baseCurrencyList)
        {
            currencyManager.AddCurrency(currency);
            expected = Arithmetic.DoubleRangeCheck(expected + currency.CurrencyValue);
        }

        List<Currency> incrementCurrencyList = CurrencyListInit(currencyType, increment);
        foreach (var currency in incrementCurrencyList)
        {
            currencyManager.RemoveCurrency(currency);
            expected = Arithmetic.DoubleRangeCheck(expected - currency.CurrencyValue);
        }

        double actual = currencyManager.CurrencyDict[currencyType].CurrencyValue;
        Assert.AreEqual(expected, actual);
    }

    [TestCase(100.0, 10.4, 200, 10)]
    [TestCase(100.5, 99.9, 2000, 100)]
    [TestCase(double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue)]
    public void CurrencyManager_AddCurrencyManager_CurrencyValueIncreased(
        double coinValue,
        double noteValue,
        double normalCrateValue,
        double specialCrateValue)
    {
        CurrencyManager currencyManager = CurrencyManagerInit();
        CurrencyManager baseCurrencyManager = CurrencyManagerInitPopulated(2000.0, 103.4, 200.0, 10.0);
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
            double expected = baseCurrencyManager.CurrencyDict[currencyType].CurrencyValue;
            expected += incrementCurrencyManager.CurrencyDict[currencyType].CurrencyValue;
            Assert.AreEqual(expected, currencyManager.CurrencyDict[currencyType].CurrencyValue);
        }
    }

    [TestCase(100.0, 10.4, 200, 10)]
    [TestCase(100.5, 99.9, 2000, 100)]
    [TestCase(double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue)]
    public void CurrencyManager_RemoveCurrencyManager_CurrencyValueDecreased(
        double coinValue,
        double noteValue,
        double normalCrateValue,
        double specialCrateValue)
    {
        CurrencyManager currencyManager = CurrencyManagerInit();
        CurrencyManager baseCurrencyManager = CurrencyManagerInitPopulated(2000.0, 103.4, 200.0, 10.0);
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
            double expected = baseCurrencyManager.CurrencyDict[currencyType].CurrencyValue;
            expected -= incrementCurrencyManager.CurrencyDict[currencyType].CurrencyValue;
            Assert.AreEqual(expected, currencyManager.CurrencyDict[currencyType].CurrencyValue);
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
            Assert.IsTrue(currencyManager.CurrencyDict[currencyType] != 
                cloneCurrencyManager.CurrencyDict[currencyType]);
        }
    }

    private CurrencyManager CurrencyManagerInit() => new();

    private List<Currency> CurrencyListInit(CurrencyType currencyType, double baseValue)
    {
        List<Currency> currencyList = new();

        for (int i = 0; i < 3; i++)
        {
            Currency currency = new(currencyType, baseValue);
            currencyList.Add(currency);
        }

        return currencyList;
    }

    private CurrencyManager CurrencyManagerInitPopulated(
        double coinValue,
        double noteValue,
        double normalCrateValue,
        double specialCrateValue)
    {
        CurrencyManager currencyManager = new();

        List<Currency> coinList = CurrencyListInit(CurrencyType.Coin, coinValue);
        List<Currency> noteList = CurrencyListInit(CurrencyType.Note, noteValue);
        List<Currency> normalCrateList = CurrencyListInit(CurrencyType.NormalCrate, normalCrateValue);
        List<Currency> specialCrateList = CurrencyListInit(CurrencyType.SpecialCrate, specialCrateValue);

        List<List<Currency>> currencies = new()
        {
            coinList,
            noteList,
            normalCrateList,
            specialCrateList
        };

        foreach (var currencyList in currencies)
        {
            foreach (var currency in currencyList)
            {
                currencyManager.AddCurrency(currency);
            }
        }

        return currencyManager;
    }
}
