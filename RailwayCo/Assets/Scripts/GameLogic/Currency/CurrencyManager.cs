using System;
using System.Collections.Generic;

public class CurrencyManager
{
    private Dictionary<CurrencyType, Currency> currencyDict;

    public CurrencyManager()
    {
        CurrencyDict = new();
        foreach (CurrencyType currencyType in Enum.GetValues(typeof(CurrencyType)))
        {
            CurrencyDict[currencyType] = new(currencyType, 0.0);
        }
    }

    public Dictionary<CurrencyType, Currency> CurrencyDict
    {
        get => currencyDict; private set => currencyDict = value;
    }

    public void AddCurrency(Currency currency)
    {
        CurrencyType currencyType = currency.CurrencyType;
        CurrencyDict[currencyType].AddCurrencyValue(currency.CurrencyValue);
    }

    public void RemoveCurrency(Currency currency)
    {
        CurrencyType currencyType = currency.CurrencyType;
        CurrencyDict[currencyType].RemoveCurrencyValue(currency.CurrencyValue);
    }

    public void AddCurrencyManager(CurrencyManager currencyManager)
    {
        List<CurrencyType> currencyTypes = new(currencyManager.CurrencyDict.Keys);
        foreach (CurrencyType currencyType in currencyTypes)
        {
            AddCurrency(currencyManager.CurrencyDict[currencyType]);
        }
    }

    public void RemoveCurrencyManager(CurrencyManager currencyManager)
    {
        List<CurrencyType> currencyTypes = new(currencyManager.CurrencyDict.Keys);
        foreach (CurrencyType currencyType in currencyTypes)
        {
            RemoveCurrency(currencyManager.CurrencyDict[currencyType]);
        }
    }
}
