using System.Collections;
using System.Collections.Generic;

public class Currency
{
    private CurrencyType currencyType;
    private Attribute<float> currencyValue;

    public CurrencyType CurrencyType { get => currencyType; set => currencyType = value; }
    public Attribute<float> CurrencyValue { get => currencyValue; set => currencyValue = value; }

    public Currency(CurrencyType currencyType, Attribute<float> currencyValue)
    {
        CurrencyType = currencyType;
        CurrencyValue = currencyValue;
    }
}

public enum CurrencyType 
{
    Coin,
    Note,
    NormalCrate,
    SpecialCrate
}
