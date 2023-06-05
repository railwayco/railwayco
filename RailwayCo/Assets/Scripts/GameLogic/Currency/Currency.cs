public class Currency
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

    public void AddCurrencyValue(double currencyValue) => CurrencyValue += currencyValue;
    public void RemoveCurrencyValue(double currencyValue) => CurrencyValue -= currencyValue;
}
