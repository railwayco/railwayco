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
        CurrencyRangedManager currencyRangedManager = new();
        currencyRangedManager.SetCurrencyRanged(CurrencyType.Coin, 100, 200);
        currencyRangedManager.SetCurrencyRanged(CurrencyType.Note, 200, 300);
        currencyRangedManager.SetCurrencyRanged(CurrencyType.NormalCrate, 5, 10);
        currencyRangedManager.SetCurrencyRanged(CurrencyType.SpecialCrate, 10, 20);

        CargoModel cargoModel = new(CargoType.Wood, 50, 100, currencyRangedManager);
        cargoModel.Randomise();

        Cargo cargo = new(cargoModel, System.Guid.NewGuid(), System.Guid.NewGuid());
        return cargo;
    }
}
