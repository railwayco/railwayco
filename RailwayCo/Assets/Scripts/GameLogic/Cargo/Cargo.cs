using System;

public class Cargo : Worker
{
    private CargoType _type;

    public override Enum Type { get => _type; protected set => _type = (CargoType)value; }
    public double Weight { get; private set; }
    public CurrencyManager CurrencyManager { get; private set; }
    public Guid SourceStationGuid { get; private set; }
    public Guid DestinationStationGuid { get; private set; }

    public Cargo(
        CargoType type,
        double weight,
        CurrencyManager currencyManager,
        Guid sourceStationGuid,
        Guid destinationStationGuid)
    {
        Guid = Guid.NewGuid();
        Type = type;
        Weight = weight;
        CurrencyManager = currencyManager;
        SourceStationGuid = sourceStationGuid;
        DestinationStationGuid = destinationStationGuid;
    }
}
