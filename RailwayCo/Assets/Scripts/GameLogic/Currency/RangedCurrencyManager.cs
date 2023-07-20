using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

public class RangedCurrencyManager : ICloneable, IEquatable<RangedCurrencyManager>
{
    [JsonProperty]
    private Dictionary<CurrencyType, IntAttribute> RangedCurrencyDict { get; set; }

    [JsonIgnore]
    public List<CurrencyType> CurrencyTypes => new(RangedCurrencyDict.Keys);

    public RangedCurrencyManager()
    {
        RangedCurrencyDict = new();
        foreach (CurrencyType currencyType in Enum.GetValues(typeof(CurrencyType)))
        {
            RangedCurrencyDict[currencyType] = new(0, 0, 0, 0);
        }
    }

    public void SetRangedCurrency(CurrencyType currencyType, int currencyLowerValue, int currencyUpperValue)
    {
        RangedCurrencyDict[currencyType] = new(currencyLowerValue, currencyUpperValue, 0, 0);
    }

    public IntAttribute GetRangedCurrency(CurrencyType currencyType)
    {
        return (IntAttribute)RangedCurrencyDict[currencyType].Clone();
    }

    public void Randomise()
    {
        Random rand = new();
        foreach (CurrencyType currencyType in CurrencyTypes)
        {
            IntAttribute currency = RangedCurrencyDict[currencyType];
            int lowerLimit = currency.LowerLimit;
            int upperLimit = currency.UpperLimit;
            currency.Amount = rand.Next(lowerLimit, upperLimit + 1);
        }
    }

    public CurrencyManager InitCurrencyManager()
    {
        CurrencyManager currencyManager = new();
        foreach (CurrencyType currencyType in CurrencyTypes)
        {
            IntAttribute currency = RangedCurrencyDict[currencyType];
            currencyManager.AddCurrency(currencyType, currency.Amount);
        }
        return currencyManager;
    }

    public object Clone()
    {
        RangedCurrencyManager currencyRangedManager = (RangedCurrencyManager)MemberwiseClone();
        currencyRangedManager.RangedCurrencyDict = new(currencyRangedManager.RangedCurrencyDict);
        List<CurrencyType> currencyTypes = currencyRangedManager.RangedCurrencyDict.Keys.ToList();
        currencyTypes.ForEach(currencyType =>
        {
            currencyRangedManager.RangedCurrencyDict[currencyType] = 
                (IntAttribute)currencyRangedManager.RangedCurrencyDict[currencyType].Clone();
        });
        return currencyRangedManager;
    }

    public bool Equals(RangedCurrencyManager other)
    {
        foreach (var keyValuePair in RangedCurrencyDict)
        {
            CurrencyType key = keyValuePair.Key;
            Attribute<int> currency = keyValuePair.Value;
            if (!RangedCurrencyDict.TryGetValue(key, out IntAttribute currencyToVerify))
                return false;
            if (!currency.Equals(currencyToVerify))
                return false;
        }
        return true;
    }
}
