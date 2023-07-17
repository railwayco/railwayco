using System;
using Newtonsoft.Json;

public class Cargo : Worker, IEquatable<Cargo>
{
    private CargoType _type;
    private CurrencyManager _currencyManager;

    public override Enum Type { get => _type; protected set => _type = (CargoType)value; }
    public int Weight { get; private set; }
    public CurrencyManager CurrencyManager
    { 
        get => (CurrencyManager)_currencyManager.Clone(); 
        private set => _currencyManager = value;
    }
    public TravelPlan TravelPlan { get; private set; }
    public CargoAssociation CargoAssoc { get; set; }

    [JsonConstructor]
    private Cargo(
        string guid,
        string type,
        int weight,
        CurrencyManager currencyManager,
        TravelPlan travelPlan,
        string cargoAssoc)
    {
        Guid = new(guid);
        Type = Enum.Parse<CargoType>(type);
        Weight = weight;
        CurrencyManager = currencyManager;
        TravelPlan = travelPlan;
        CargoAssoc = Enum.Parse<CargoAssociation>(cargoAssoc);
    }

    public Cargo(
        CargoModel cargoModel,
        Guid sourceStation,
        Guid destinationStation)
    {
        Guid = Guid.NewGuid();
        Type = (CargoType)cargoModel.Type;
        Weight = cargoModel.Weight.Amount;
        CurrencyManager = cargoModel.RangedCurrencyManager.InitCurrencyManager();
        TravelPlan = new(sourceStation, destinationStation);
        CargoAssoc = CargoAssociation.Nil;
    }

    public override object Clone()
    {
        Cargo cargo = (Cargo)MemberwiseClone();

        cargo.CurrencyManager = (CurrencyManager)cargo.CurrencyManager.Clone();

        return cargo;
    }

    public bool Equals(Cargo other)
    {
        return Type.Equals(other.Type)
            && Weight.Equals(other.Weight)
            && CurrencyManager.Equals(other.CurrencyManager)
            && TravelPlan.Equals(other.TravelPlan)
            && CargoAssoc.Equals(other.CargoAssoc);
    }
}
