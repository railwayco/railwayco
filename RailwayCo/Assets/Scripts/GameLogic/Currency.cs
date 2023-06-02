using System.Collections;
using System.Collections.Generic;

public class Currency
{
    private CurrencyType currencyType;
    private Attribute<float> currencyValue;

    public CurrencyType CurrencyType { get => currencyType; set => currencyType = value; }
    public Attribute<float> CurrencyValue { get => currencyValue; private set => currencyValue = value; }

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

public class CurrencyManager
{
    private Dictionary<CurrencyType, Attribute<float>> currencyDict;

    public Dictionary<CurrencyType, Attribute<float>> CurrencyDict
    {
        get => currencyDict; private set
        {
            currencyDict = value;
        }
    }

    public Dictionary<string, string> AddCurrency(Currency currency)
    {
        CurrencyType currencyType = currency.CurrencyType;
        Dictionary<string, string> result = new()
        {
            { "old", currencyDict[currencyType].Amount.ToString() }
        };

        currencyDict[currencyType].Amount += currency.CurrencyValue.Amount;
        result.Add("new", currencyDict[currencyType].Amount.ToString());

        return result;
    }

    public Dictionary<string, string> RemoveCurrency(Currency currency)
    {
        CurrencyType currencyType = currency.CurrencyType;
        Dictionary<string, string> result = new()
        {
            { "old", currencyDict[currencyType].Amount.ToString() }
        };

        currencyDict[currencyType].Amount -= currency.CurrencyValue.Amount;
        result.Add("new", currencyDict[currencyType].Amount.ToString());

        return result;
    }
}
