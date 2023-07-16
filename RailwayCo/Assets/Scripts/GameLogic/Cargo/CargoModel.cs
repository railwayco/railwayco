using System;
using Newtonsoft.Json;

public class CargoModel : Worker, IEquatable<CargoModel>
{
    private CargoType _type;

    public override Enum Type { get => _type; protected set => _type = (CargoType)value; }
    public DoubleAttribute Weight { get; private set; }
    public CurrencyRangedManager CurrencyRangedManager { get; private set; }

    [JsonConstructor]
    private CargoModel(
        string guid,
        string type,
        DoubleAttribute weight,
        CurrencyRangedManager currencyRangedManager)
    {
        Guid = new(guid);
        Type = Enum.Parse<CargoType>(type);
        Weight = weight;
        CurrencyRangedManager = currencyRangedManager;
    }

    public CargoModel(
        CargoType type,
        int weightLowerLimit,
        int weightUpperLimit,
        CurrencyRangedManager currencyRangedManager)
    {
        Guid = Guid.NewGuid();
        Type = type;
        Weight = new(weightLowerLimit, weightUpperLimit, 0, 0);
        CurrencyRangedManager = currencyRangedManager;
    }

    public void Randomise()
    {
        Random rand = new();
        int lowerLimit = Weight.LowerLimit;
        int upperLimit = Weight.UpperLimit;
        Weight.Amount = rand.Next() * (upperLimit - lowerLimit) + lowerLimit;
        CurrencyRangedManager.Randomise();
    }

    public override object Clone()
    {
        CargoModel cargoModel = (CargoModel)MemberwiseClone();

        cargoModel.Weight = (DoubleAttribute)cargoModel.Weight.Clone();
        cargoModel.CurrencyRangedManager = (CurrencyRangedManager)cargoModel.CurrencyRangedManager.Clone();

        return cargoModel;
    }

    public bool Equals(CargoModel other)
    {
        return Type.Equals(other.Type)
            && Weight.Equals(other.Weight)
            && CurrencyRangedManager.Equals(other.CurrencyRangedManager);
    }
}
