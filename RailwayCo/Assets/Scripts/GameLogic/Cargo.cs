using System.Collections;

public class Cargo
{
    private CargoType cargoType;
    private Attribute<float> weight;
    private Currency currency;
    private Station sourceStation;
    private Station destinationStation;

    public CargoType CargoType { get => cargoType; private set => cargoType = value; }
    public Attribute<float> Weight { get => weight; private set => weight = value; }
    public Currency Currency { get => currency; private set => currency = value; }
    public Station SourceStation { get => sourceStation; private set => sourceStation = value; }
    public Station DestinationStation { get => destinationStation; private set => destinationStation = value; }

    public Cargo(
        CargoType cargoType,
        Attribute<float> weight,
        Currency currency,
        Station sourceStation,
        Station destinationStation)
    {
        CargoType = cargoType;
        Weight = weight;
        Currency = currency;
        SourceStation = sourceStation;
        DestinationStation = destinationStation;
    }
}
