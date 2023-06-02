using System.Collections;
using System.Collections.Generic;

public class Cargo
{
    private CargoType cargoType;
    private Attribute<float> weight;
    private Currency currency;

    public CargoType CargoType { get => cargoType; private set => cargoType = value; }
    public Attribute<float> Weight { get => weight; private set => weight = value; }
    public Currency Currency { get => currency; private set => currency = value; }

    public Cargo(CargoType cargoType, Attribute<float> weight, Currency currency)
    {
        CargoType = cargoType;
        Weight = weight;
        Currency = currency;
    }
}

public enum CargoType
{
    Wood,
    Crate
}
