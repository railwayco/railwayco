using System;
using Newtonsoft.Json;

public class CargoModel : Worker, IEquatable<CargoModel>
{
    private CargoType _type;

    public override Enum Type { get => _type; protected set => _type = (CargoType)value; }
    public Attribute<double> Weight { get; private set; }
    public CurrencyManager CurrencyManager { get; private set; }

    [JsonConstructor]
    private CargoModel(
        string guid,
        string type,
        Attribute<double> weight,
        CurrencyManager currencyManager)
    {
        Guid = new(guid);
        Type = Enum.Parse<CargoType>(type);
        Weight = weight;
        CurrencyManager = currencyManager;
    }

    public CargoModel(
        CargoType type,
        double weightLowerLimit,
        double weightUpperLimit,
        CurrencyManager currencyManager)
    {
        Guid = Guid.NewGuid();
        Type = type;
        Weight = new(weightLowerLimit, weightUpperLimit, double.NaN, 0);
        CurrencyManager = currencyManager;
    }

    public void Randomise()
    {
        Random rand = new();
        double lowerLimit = Weight.LowerLimit;
        double upperLimit = Weight.UpperLimit;
        Weight.Amount = rand.NextDouble() * (upperLimit - lowerLimit) + lowerLimit;

        // TODO: Randomise rewards
    }

    public override object Clone()
    {
        CargoModel cargoModel = (CargoModel)MemberwiseClone();

        cargoModel.Weight = (Attribute<double>)cargoModel.Weight.Clone();
        cargoModel.CurrencyManager = (CurrencyManager)cargoModel.CurrencyManager.Clone();

        return cargoModel;
    }

    public bool Equals(CargoModel other)
    {
        return Type.Equals(other.Type)
            && Weight.Equals(other.Weight)
            && CurrencyManager.Equals(other.CurrencyManager);
    }
}
