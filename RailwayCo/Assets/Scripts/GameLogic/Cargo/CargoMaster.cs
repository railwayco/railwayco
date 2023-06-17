using System;

public class CargoMaster : Master<Cargo>
{
    public CargoMaster() => Collection = new();

    public Cargo Init(
        CargoType type,
        double weight,
        CurrencyManager currencyManager,
        Guid sourceStationGuid,
        Guid destinationStationGuid)
    {
        return new(
            type,
            weight,
            currencyManager,
            sourceStationGuid,
            destinationStationGuid);
    }

    public void AddCargo(Cargo cargo) => Add(cargo.Guid, cargo);
    public void RemoveCargo(Guid guid) => Remove(guid);
}
