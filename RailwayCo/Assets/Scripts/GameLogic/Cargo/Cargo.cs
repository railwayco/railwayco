using System;

public class Cargo : Worker
{
    private CargoType _type;

    public override Enum Type { get => _type; protected set => _type = (CargoType)value; }
    public double Weight { get; private set; }
    public CurrencyManager CurrencyManager { get; private set; }
    public Station SourceStation { get; private set; }
    public Station DestinationStation { get; private set; }

    public Cargo(
        CargoType type,
        double weight,
        CurrencyManager currencyManager,
        Station sourceStation,
        Station destinationStation)
    {
        Guid = Guid.NewGuid();
        Type = type;
        Weight = weight;
        CurrencyManager = currencyManager;
        SourceStation = sourceStation;
        DestinationStation = destinationStation;
    }
}
