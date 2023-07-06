using System;

public class Currency : ICloneable
{
    public CurrencyType CurrencyType { get; set; }
    public double CurrencyValue { get; private set; }

    public Currency(CurrencyType currencyType, double currencyValue)
    {
        CurrencyType = currencyType;
        CurrencyValue = currencyValue;
    }

    public void AddCurrencyValue(double currencyValue)
    {
        if (currencyValue < 0.0) throw new ArgumentException("Invalid currency value");
        CurrencyValue = Arithmetic.DoubleRangeCheck(CurrencyValue + currencyValue);
    }

    public void RemoveCurrencyValue(double currencyValue)
    {
        if (currencyValue < 0.0) throw new ArgumentException("Invalid currency value");
        CurrencyValue = Arithmetic.DoubleRangeCheck(CurrencyValue - currencyValue);
    }

    public object Clone()
    {
        Currency currency = new(CurrencyType, CurrencyValue);
        return currency;
    }
}
