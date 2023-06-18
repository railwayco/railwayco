using System;

public class CargoCatalog : Catalog<CargoModel>
{
    public CargoCatalog() => Collection = new();

    public CargoModel Init(
        CargoType type,
        double weightLowerLimit,
        double weightUpperLimit,
        CurrencyManager currencyManager)
    {
        return new(
            type,
            weightLowerLimit,
            weightUpperLimit,
            currencyManager);
    }

    public void AddCargoModel(CargoModel cargoModel) => Add(cargoModel.Guid, cargoModel);
    public void RemoveCargoModel(Guid guid) => Remove(guid);
}
