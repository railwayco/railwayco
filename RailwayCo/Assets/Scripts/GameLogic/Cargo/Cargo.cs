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
        Guid sourceStation,
        Guid destinationStation)
    {
        Guid = Guid.NewGuid();
        Type = type;
        Weight = weight;
        CurrencyManager = currencyManager;
        TravelPlan = new(sourceStation, destinationStation);
    }

    public bool HasArrived(Guid station) => TravelPlan.HasArrived(station);
    public bool IsAtSource(Guid station) => TravelPlan.IsAtSource(station);
    public Guid GetDestination() => TravelPlan.DestinationStation;

    public override object Clone()
    {
        Cargo cargo = (Cargo)this.MemberwiseClone();

        CurrencyManager currencyManager = new();
        currencyManager.AddCurrencyManager(cargo.CurrencyManager);
        cargo.CurrencyManager = currencyManager;

        return cargo;
    }
}
