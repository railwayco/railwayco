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
        RangedCurrencyManager rangedCurrencyManager = new();
        rangedCurrencyManager.SetRangedCurrency(CurrencyType.Coin, 100, 200);
        rangedCurrencyManager.SetRangedCurrency(CurrencyType.Note, 200, 300);
        rangedCurrencyManager.SetRangedCurrency(CurrencyType.NormalCrate, 5, 10);
        rangedCurrencyManager.SetRangedCurrency(CurrencyType.SpecialCrate, 10, 20);

        CargoModel cargoModel = new(CargoType.Wood, 50, 100, rangedCurrencyManager);
        cargoModel.Randomise();

        Cargo cargo = new(cargoModel, System.Guid.NewGuid(), System.Guid.NewGuid());
        return cargo;
    }
}
