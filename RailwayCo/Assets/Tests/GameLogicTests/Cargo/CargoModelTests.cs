﻿using NUnit.Framework;

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
    public void CargoModel_Randomise_IsWeightAmountSet()
    {
        CargoModel cargoModel = CargoModelInit();
        Assert.AreEqual(0, cargoModel.Weight.Amount);
        cargoModel.Randomise();
        Assert.AreNotEqual(0, cargoModel.Weight.Amount);
    }

    [Test]
    public void CargoModel_Clone_IsDeepCopy()
    {
        CargoModel cargoModel = CargoModelInit();
        CargoModel cargoModelClone = (CargoModel)cargoModel.Clone();

        cargoModelClone.Randomise();
        cargoModelClone.CurrencyManager.AddCurrency(CurrencyType.Coin, 100);

        Assert.AreNotEqual(cargoModel, cargoModelClone);
    }

    private CargoModel CargoModelInit()
    {
        CurrencyManager currencyManager = new();
        currencyManager.AddCurrency(CurrencyType.Coin, 100);
        currencyManager.AddCurrency(CurrencyType.Note, 200);
        currencyManager.AddCurrency(CurrencyType.NormalCrate, 5);
        currencyManager.AddCurrency(CurrencyType.SpecialCrate, 10);

        CargoModel cargoModel = new(CargoType.Wood, 50, 100, currencyManager);
        return cargoModel;
    }
}
