using System;
using System.Collections.Generic;

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
    public void RemoveCargoRange(HashSet<Guid> guids)
    {
        foreach (var guid in guids) RemoveCargo(guid);
    }
    public HashSet<Guid> FilterCargoHasArrived(HashSet<Guid> cargos, Guid station)
    {
        foreach(Guid cargo in cargos)
        {
            if (!Get(cargo).HasArrived(station)) cargos.Remove(cargo);
        }
        return cargos;
    }
    public CurrencyManager GetCurrencyManagerForCargoRange(HashSet<Guid> guids)
    {
        CurrencyManager total = new();
        foreach(var guid in guids) total.AddCurrencyManager(Get(guid).CurrencyManager);
        return total;
    }
}
