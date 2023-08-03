using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class CurrencyManager : ICloneable, IEquatable<CurrencyManager>
{
    [JsonProperty]
    private Dictionary<CurrencyType, int> CurrencyDict { get; set; }

    [JsonIgnore]
    public List<CurrencyType> CurrencyTypes => new(CurrencyDict.Keys);

    public CurrencyManager()
    {
        CurrencyDict = new();
        foreach (CurrencyType currencyType in Enum.GetValues(typeof(CurrencyType)))
        {
            CurrencyDict[currencyType] = 0;
        }
    }

    public int GetCurrency(CurrencyType currencyType) => CurrencyDict.GetValueOrDefault(currencyType);

    public void AddCurrency(CurrencyType currencyType, int currencyValue)
    {
        CurrencyDict[currencyType] = Arithmetic.IntAddition(CurrencyDict[currencyType], currencyValue);
    }

    public void RemoveCurrency(CurrencyType currencyType, int currencyValue)
    {
        CurrencyDict[currencyType] = Arithmetic.IntSubtraction(CurrencyDict[currencyType], currencyValue);
    }

    public void AddCurrencyManager(CurrencyManager currencyManager)
    {
        foreach (var currencyType in CurrencyTypes)
        {
            int currencyValue = currencyManager.CurrencyDict[currencyType];
            AddCurrency(currencyType, currencyValue);
        }
    }

    public void RemoveCurrencyManager(CurrencyManager currencyManager)
    {
        foreach (var currencyType in CurrencyTypes)
        {
            int currencyValue = currencyManager.CurrencyDict[currencyType];
            RemoveCurrency(currencyType, currencyValue);
        }
    }

    public object Clone()
    {
        CurrencyManager currencyManager = (CurrencyManager)MemberwiseClone();
        currencyManager.CurrencyDict = new(currencyManager.CurrencyDict);
        return currencyManager;
    }

    public bool Equals(CurrencyManager other)
    {
        foreach (var keyValuePair in CurrencyDict)
        {
            CurrencyType key = keyValuePair.Key;
            int currencyValue = keyValuePair.Value;
            if (!currencyValue.Equals(other.CurrencyDict.GetValueOrDefault(key)))
                return false;
        }
        return true;
    }
}
