using NUnit.Framework;

public class CargoTests
{
    [Test]
    public void Cargo_Cargo_IsJsonSerialisedCorrectly()
    {
        Cargo cargo = CargoInit();

        string jsonString = GameDataManager.Serialize(cargo);
        Cargo cargoToVerify = GameDataManager.Deserialize<Cargo>(jsonString);

        Assert.AreEqual(cargo, cargoToVerify);
    }

    private Cargo CargoInit()
    {
        CurrencyManager currencyManager = new();
        currencyManager.AddCurrency(CurrencyType.Coin, 100);
        currencyManager.AddCurrency(CurrencyType.Note, 200);
        currencyManager.AddCurrency(CurrencyType.NormalCrate, 5);
        currencyManager.AddCurrency(CurrencyType.SpecialCrate, 10);

        CargoModel cargoModel = new(CargoType.Wood, 50, 100, currencyManager);
        cargoModel.Randomise();

        Cargo cargo = new(cargoModel, System.Guid.NewGuid(), System.Guid.NewGuid());
        return cargo;
    }
}
