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
        currencyManager.AddCurrency(new(CurrencyType.Coin, 100));
        currencyManager.AddCurrency(new(CurrencyType.Note, 200));
        currencyManager.AddCurrency(new(CurrencyType.NormalCrate, 5));
        currencyManager.AddCurrency(new(CurrencyType.SpecialCrate, 10));

        CargoModel cargoModel = new(CargoType.Wood, 50, 100, currencyManager);
        cargoModel.Randomise();

        Cargo cargo = new(cargoModel, System.Guid.NewGuid(), System.Guid.NewGuid());
        return cargo;
    }
}
