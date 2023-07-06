using System;
using System.Collections.Generic;

public class CurrencyManager : ICloneable
{
    public Dictionary<CurrencyType, Currency> CurrencyDict { get; private set; }

    public CurrencyManager()
    {
        CurrencyDict = new();
        foreach (CurrencyType currencyType in Enum.GetValues(typeof(CurrencyType)))
        {
            CurrencyDict[currencyType] = new(currencyType, 0.0);
        }
    }

    public void AddCurrency(Currency currency)
    {
        CurrencyType currencyType = currency.CurrencyType;
        CurrencyDict[currencyType].AddCurrencyValue((double)currency.CurrencyValue);
    }

    public void RemoveCurrency(Currency currency)
    {
        CurrencyType currencyType = currency.CurrencyType;
        CurrencyDict[currencyType].RemoveCurrencyValue((double)currency.CurrencyValue);
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

    public object Clone()
    {
        CurrencyManager currencyManager = new();
        foreach (CurrencyType currencyType in Enum.GetValues(typeof(CurrencyType)))
        {
            currencyManager.CurrencyDict[currencyType] = (Currency)CurrencyDict[currencyType].Clone();
        }
        return currencyManager;
    }
}
