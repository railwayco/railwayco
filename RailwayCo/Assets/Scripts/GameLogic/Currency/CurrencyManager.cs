using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

public class CurrencyManager : ICloneable, IEquatable<CurrencyManager>
{
    public Dictionary<CurrencyType, int> CurrencyDict { get; private set; }

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

    public int GetCurrency(CurrencyType currencyType)
    {
        CurrencyDict.TryGetValue(currencyType, out int currencyValue);
        return currencyValue;
    }

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
        foreach (var currencyType in currencyManager.CurrencyDict.Keys.ToList())
        {
            int currencyValue = currencyManager.CurrencyDict[currencyType];
            AddCurrency(currencyType, currencyValue);
        }
    }

    public void RemoveCurrencyManager(CurrencyManager currencyManager)
    {
        foreach (var currencyType in currencyManager.CurrencyDict.Keys.ToList())
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
            if (!CurrencyDict.TryGetValue(key, out int valueToVerify))
                return false;
            if (!currencyValue.Equals(valueToVerify))
                return false;
        }
        return true;
    }
}
