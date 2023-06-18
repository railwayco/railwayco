using System;
using System.Collections.Generic;

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

    public CargoModel GetRandomCargoModel()
    {
        List<Guid> keys = new(Collection.Keys);

        Random rand = new();
        int randomIndex = rand.Next(keys.Count);

        Guid randomGuid = keys[randomIndex];
        return (CargoModel)Get(randomGuid).Clone();
    }
}
