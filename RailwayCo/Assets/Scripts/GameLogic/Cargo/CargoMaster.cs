using System;

public class CargoMaster : Master<Cargo>
{
    public CargoMaster() => Collection = new();

    public Cargo Init() => new();
    public void AddCargo(Cargo cargo) => Add(cargo.Guid, cargo);
    public void RemoveCargo(Guid guid) => Remove(guid);
}
