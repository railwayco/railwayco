using System;
using System.Collections.Generic;
using System.Linq;

public class CurrencyRangedManager : ICloneable, IEquatable<CurrencyRangedManager>
{
    public Dictionary<CurrencyType, Attribute<int>> CurrencyRangedDict { get; private set; }

    public CurrencyRangedManager()
    {
        CurrencyRangedDict = new();
        foreach (CurrencyType currencyType in Enum.GetValues(typeof(CurrencyType)))
        {
            CurrencyRangedDict[currencyType] = new(0, 0, 0, 0);
        }
    }

    public void SetCurrencyRanged(CurrencyType currencyType, int currencyLowerValue, int currencyUpperValue)
    {
        CurrencyRangedDict[currencyType] = new(currencyLowerValue, currencyUpperValue, 0, 0);
    }

    public void Randomise()
    {
        Random rand = new();
        foreach (CurrencyType currencyType in Enum.GetValues(typeof(CurrencyType)))
        {
            Attribute<int> currency = CurrencyRangedDict[currencyType];
            int lowerLimit = currency.LowerLimit;
            int upperLimit = currency.UpperLimit;
            currency.Amount = rand.Next() * (upperLimit - lowerLimit) + lowerLimit;
        }
    }

    public CurrencyManager InitCurrencyManager()
    {
        CurrencyManager currencyManager = new();
        foreach (CurrencyType currencyType in Enum.GetValues(typeof(CurrencyType)))
        {
            Attribute<int> currency = CurrencyRangedDict[currencyType];
            currencyManager.AddCurrency(currencyType, currency.Amount);
        }
        return currencyManager;
    }

    public object Clone()
    {
        CurrencyRangedManager currencyRangedManager = (CurrencyRangedManager)MemberwiseClone();
        currencyRangedManager.CurrencyRangedDict = new(currencyRangedManager.CurrencyRangedDict);
        List<CurrencyType> currencyTypes = currencyRangedManager.CurrencyRangedDict.Keys.ToList();
        currencyTypes.ForEach(currencyType =>
        {
            currencyRangedManager.CurrencyRangedDict[currencyType] = 
                (Attribute<int>)currencyRangedManager.CurrencyRangedDict[currencyType].Clone();
        });
        return currencyRangedManager;
    }

    public bool Equals(CurrencyRangedManager other)
    {
        foreach (var keyValuePair in CurrencyRangedDict)
        {
            CurrencyType key = keyValuePair.Key;
            Attribute<int> currency = keyValuePair.Value;
            if (!CurrencyRangedDict.TryGetValue(key, out Attribute<int> currencyToVerify))
                return false;
            if (!currency.Equals(currencyToVerify))
                return false;
        }
        return true;
    }
}
