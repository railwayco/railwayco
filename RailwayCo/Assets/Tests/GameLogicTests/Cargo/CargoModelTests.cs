using NUnit.Framework;
using System;
using System.Collections.Generic;

public class CargoModelTests
{
    [Test]
    public void CargoModel_CargoModel_IsJsonSerialisedCorrectly()
    {
        CargoModel cargoModel = CargoModelInit();

        string jsonString = GameDataManager.Serialize(cargoModel);
        CargoModel cargoModelToVerify = GameDataManager.Deserialize<CargoModel>(jsonString);

        Assert.AreEqual(cargoModel, cargoModelToVerify);
    }

    [Test]
    public void CargoModel_Randomise_IsWeightAndCurrencyAmountSet()
    {
        CargoModel cargoModel = CargoModelInit();
        CurrencyType[] currencyTypes = (CurrencyType[])Enum.GetValues(typeof(CurrencyType));

        Assert.AreEqual(0, cargoModel.Weight.Amount);
        foreach (var currencyType in currencyTypes)
        {
            IntAttribute rangedCurrency = cargoModel.RangedCurrencyManager.GetRangedCurrency(currencyType);
            Assert.AreEqual(0, rangedCurrency.Amount);
        }

        cargoModel.Randomise();
        Assert.AreNotEqual(0, cargoModel.Weight.Amount);
        HashSet<int> allCurrencyAmounts = new();
        foreach (var currencyType in currencyTypes)
        {
            IntAttribute rangedCurrency = cargoModel.RangedCurrencyManager.GetRangedCurrency(currencyType);
            allCurrencyAmounts.Add(rangedCurrency.Amount);
        }
        allCurrencyAmounts.Remove(0);
        Assert.IsNotEmpty(allCurrencyAmounts);
    }

    [Test]
    public void CargoModel_Clone_IsDeepCopy()
    {
        CargoModel cargoModel = CargoModelInit();
        CargoModel cargoModelClone = (CargoModel)cargoModel.Clone();

        cargoModelClone.Randomise();

        Assert.AreNotEqual(cargoModel, cargoModelClone);
    }

    private CargoModel CargoModelInit()
    {
        RangedCurrencyManager currencyRangedManager = new();
        currencyRangedManager.SetRangedCurrency(CurrencyType.Coin, 100, 200);
        currencyRangedManager.SetRangedCurrency(CurrencyType.Note, 200, 300);
        currencyRangedManager.SetRangedCurrency(CurrencyType.NormalCrate, 5, 10);
        currencyRangedManager.SetRangedCurrency(CurrencyType.SpecialCrate, 10, 20);

        CargoModel cargoModel = new(CargoType.Wood, 50, 100, currencyRangedManager);
        return cargoModel;
    }
}
