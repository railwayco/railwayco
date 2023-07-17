using System;
using System.Collections.Generic;
using System.Linq;

public class CurrencyManagerRandomGenerator : ICloneable, IEquatable<CurrencyManagerRandomGenerator>
{
    public Dictionary<CurrencyType, IntAttribute> CurrencyRangedDict { get; private set; }

    public CurrencyManagerRandomGenerator()
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
        CurrencyManagerRandomGenerator currencyManager = (CurrencyManagerRandomGenerator)MemberwiseClone();
        currencyManager.CurrencyRangedDict = new(currencyManager.CurrencyRangedDict);
        List<CurrencyType> currencyTypes = currencyManager.CurrencyRangedDict.Keys.ToList();
        currencyTypes.ForEach(currencyType =>
        {
            currencyManager.CurrencyRangedDict[currencyType] = 
                (IntAttribute)currencyManager.CurrencyRangedDict[currencyType].Clone();
        });
        return currencyManager;
    }

    public bool Equals(CurrencyManagerRandomGenerator other)
    {
        foreach (var keyValuePair in CurrencyRangedDict)
        {
            CurrencyType key = keyValuePair.Key;
            Attribute<int> currency = keyValuePair.Value;
            if (!CurrencyRangedDict.TryGetValue(key, out IntAttribute currencyToVerify))
                return false;
            if (!currency.Equals(currencyToVerify))
                return false;
        }
        return true;
    }
}
