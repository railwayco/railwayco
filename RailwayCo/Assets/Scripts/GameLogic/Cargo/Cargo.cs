using System;

public class Cargo : Worker
{
    private CargoType _type;

    public override Enum Type { get => _type; protected set => _type = (CargoType)value; }
    public double Weight { get; private set; }
    public CurrencyManager CurrencyManager { get; private set; }
    public TravelPlan TravelPlan { get; private set; }
    public CargoAssociation CargoAssoc { get; private set; }

    public Cargo(
        CargoType type,
        double weight,
        CurrencyManager currencyManager,
        Guid sourceStation,
        Guid destinationStation,
        CargoAssociation cargoAssoc
        )
    {
        Guid = Guid.NewGuid();
        Type = type;
        Weight = weight;
        CurrencyManager = currencyManager;
        TravelPlan = new(sourceStation, destinationStation);
        CargoAssoc = cargoAssoc;
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
        CargoAssoc = CargoAssociation.NIL;
    }

    public void SetCargoAssoc(CargoAssociation cargoAssociation) => CargoAssoc = cargoAssociation;

    public override object Clone()
    {
        Cargo cargo = (Cargo)this.MemberwiseClone();

        CurrencyManager currencyManager = new();
        currencyManager.AddCurrencyManager(cargo.CurrencyManager);
        cargo.CurrencyManager = currencyManager;

        return cargo;
    }
}
