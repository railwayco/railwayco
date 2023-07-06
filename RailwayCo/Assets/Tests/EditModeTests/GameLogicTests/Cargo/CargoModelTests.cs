using NUnit.Framework;

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
        Assert.AreEqual(double.NaN, cargoModel.Weight.Amount);
        cargoModel.Randomise();
        Assert.AreNotEqual(double.NaN, cargoModel.Weight.Amount);
    }
    
    private CargoModel CargoModelInit()
    {
        CurrencyManager currencyManager = new();
        currencyManager.AddCurrency(new(CurrencyType.Coin, 100));
        currencyManager.AddCurrency(new(CurrencyType.Note, 200));
        currencyManager.AddCurrency(new(CurrencyType.NormalCrate, 5));
        currencyManager.AddCurrency(new(CurrencyType.SpecialCrate, 10));

        CargoModel cargoModel = new(CargoType.Wood, 50, 100, currencyManager);
        return cargoModel;
    }
}
