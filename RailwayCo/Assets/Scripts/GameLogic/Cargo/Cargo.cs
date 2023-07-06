using System;
using Newtonsoft.Json;

public class Cargo : Worker
{
    private CargoType _type;

    public override Enum Type { get => _type; protected set => _type = (CargoType)value; }
    public double Weight { get; private set; }
    public CurrencyManager CurrencyManager { get; private set; }
    public TravelPlan TravelPlan { get; private set; }
    public CargoAssociation CargoAssoc { get; set; }

    [JsonConstructor]
    private Cargo(
        string guid,
        string type,
        double weight,
        CurrencyManager currencyManager,
        TravelPlan travelPlan)
    {
        Guid = new(guid);
        Type = Enum.Parse<CargoType>(type);
        Weight = weight;
        CurrencyManager = currencyManager;
        TravelPlan = travelPlan;
    }

    public Cargo(
        CargoModel cargoModel,
        Guid sourceStation,
        Guid destinationStation)
    {
        Guid = Guid.NewGuid();
        Type = (CargoType)cargoModel.Type;
        Weight = cargoModel.Weight.Amount;
        CurrencyManager = cargoModel.CurrencyManager;
        TravelPlan = new(sourceStation, destinationStation);
        CargoAssoc = CargoAssociation.Nil;
    }

    public override object Clone()
    {
        Cargo cargo = (Cargo)MemberwiseClone();

        cargo.CurrencyManager = (CurrencyManager)cargo.CurrencyManager.Clone();
        cargo.TravelPlan = (TravelPlan)cargo.TravelPlan.Clone();

        return cargo;
    }
}
