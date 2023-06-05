using System.Collections.Generic;

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
