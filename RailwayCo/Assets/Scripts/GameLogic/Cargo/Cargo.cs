using System;

public class Cargo : Worker
{
    private CargoType _type;

    public override Enum Type { get => _type; protected set => _type = (CargoType)value; }
    public double Weight { get; private set; }
    public CurrencyManager CurrencyManager { get; private set; }
    private TravelPlan TravelPlan { get; set; }

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
        TravelPlan = new(sourceStationGuid, destinationStationGuid);
    }

    public bool HasArrived(Guid stationGuid) => TravelPlan.HasArrived(stationGuid);
    public bool IsAtSource(Guid stationGuid) => TravelPlan.IsAtSource(stationGuid);
    public Guid GetDestination() => TravelPlan.DestinationStationGuid;
}
