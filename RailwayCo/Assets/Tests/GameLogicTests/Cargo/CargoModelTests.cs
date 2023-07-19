﻿using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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
        List<CurrencyType> currencyTypes = cargoModel.RangedCurrencyManager.CurrencyRangedDict.Keys.ToList();

        Assert.AreEqual(0, cargoModel.Weight.Amount);
        currencyTypes.ForEach(currencyType =>
        {
            Assert.AreEqual(0, cargoModel.RangedCurrencyManager.CurrencyRangedDict[currencyType].Amount);
        });

        cargoModel.Randomise();
        Assert.AreNotEqual(0, cargoModel.Weight.Amount);
        currencyTypes.ForEach(currencyType =>
        {
            Assert.AreNotEqual(0, cargoModel.RangedCurrencyManager.CurrencyRangedDict[currencyType].Amount);
        });
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
