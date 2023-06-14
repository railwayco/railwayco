public class Currency : OverflowManager
{
    private CurrencyType currencyType;
    private double currencyValue;

    public CurrencyType CurrencyType { get => currencyType; set => currencyType = value; }
    public double CurrencyValue { get => currencyValue; private set => currencyValue = value; }

    public Currency(CurrencyType currencyType, double currencyValue)
    {
        CurrencyType = currencyType;
        CurrencyValue = currencyValue;
    }

    public void AddCurrencyValue(double currencyValue)
    {
        if (currencyValue < 0.0) throw new System.ArgumentException("Invalid currency value");
        double newCurrencyValue = CurrencyValue + currencyValue;
        CurrencyValue = DoubleArithmetic(newCurrencyValue);
    }

    public void RemoveCurrencyValue(double currencyValue)
    {
        if (currencyValue < 0.0) throw new System.ArgumentException("Invalid currency value");
        double newCurrencyValue = CurrencyValue - currencyValue;
        CurrencyValue = DoubleArithmetic(newCurrencyValue);
    }
}
