using System;
using NUnit.Framework;

public class CurrencyTests
{
    [TestCase(0.0, 50.5)]
    [TestCase(double.MaxValue, 1)]
    public void Currency_AddCurrencyValue_CurrencyValueIncreased(double baseValue, double currencyValue)
    {
        Currency currency = CurrencyInit(baseValue);
        currency.AddCurrencyValue(currencyValue);
        Assert.AreEqual(currency.DoubleArithmetic(baseValue + currencyValue), currency.CurrencyValue);
    }

    [TestCase(0, -50.5)]
    [TestCase(double.MinValue, -1)]
    public void Currency_AddCurrencyValue_CurrencyValueInvalid(double baseValue, double currencyValue)
    {
        Currency currency = CurrencyInit(baseValue);
        Assert.Catch<ArgumentException>(() => currency.AddCurrencyValue(currencyValue));
    }

    [TestCase(0.0, 50.5)]
    [TestCase(double.MaxValue, 1)]
    public void Currency_RemoveCurrencyValue_CurrencyValueDecreased(double baseValue, double currencyValue)
    {
        Currency currency = CurrencyInit(baseValue);
        currency.RemoveCurrencyValue(currencyValue);
        Assert.AreEqual(currency.DoubleArithmetic(baseValue - currencyValue), currency.CurrencyValue);
    }

    [TestCase(0, -50.5)]
    [TestCase(double.MinValue, -1)]
    public void Currency_RemoveCurrencyValue_CurrencyValueInvalid(double baseValue, double currencyValue)
    {
        Currency currency = CurrencyInit(baseValue);
        Assert.Catch<ArgumentException>(() => currency.RemoveCurrencyValue(currencyValue));
    }

    public Currency CurrencyInit(double baseValue)
    {
        return new(CurrencyType.Coin, baseValue);
    }
}
