public class Currency : Arithmetic
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
        if (currencyValue < 0.0) throw new System.ArgumentException("Invalid currency value");
        CurrencyValue = DoubleRangeCheck(CurrencyValue + currencyValue);
    }

    public void RemoveCurrencyValue(double currencyValue)
    {
        if (currencyValue < 0.0) throw new System.ArgumentException("Invalid currency value");
        CurrencyValue = DoubleRangeCheck(CurrencyValue - currencyValue);
    }
}
