using System;
using Newtonsoft.Json;

public class CargoModel : Worker, IEquatable<CargoModel>
{
    private CargoType _type;

    public override Enum Type { get => _type; protected set => _type = (CargoType)value; }
    public IntAttribute Weight { get; private set; }
    public CurrencyManagerRandomGenerator CurrencyManager { get; private set; }

    [JsonConstructor]
    private CargoModel(
        string guid,
        string type,
        IntAttribute weight,
        CurrencyManagerRandomGenerator currencyManager)
    {
        Guid = new(guid);
        Type = Enum.Parse<CargoType>(type);
        Weight = weight;
        CurrencyManager = currencyManager;
    }

    public CargoModel(
        CargoType type,
        int weightLowerLimit,
        int weightUpperLimit,
        CurrencyManagerRandomGenerator currencyManager)
    {
        Guid = Guid.NewGuid();
        Type = type;
        Weight = new(weightLowerLimit, weightUpperLimit, 0, 0);
        CurrencyManager = currencyManager;
    }

    public void Randomise()
    {
        Random rand = new();
        int lowerLimit = Weight.LowerLimit;
        int upperLimit = Weight.UpperLimit;
        Weight.Amount = rand.Next() * (upperLimit - lowerLimit) + lowerLimit;
        CurrencyManager.Randomise();
    }

    public override object Clone()
    {
        CargoModel cargoModel = (CargoModel)MemberwiseClone();

        cargoModel.Weight = (IntAttribute)cargoModel.Weight.Clone();
        cargoModel.CurrencyManager = (CurrencyManagerRandomGenerator)cargoModel.CurrencyManager.Clone();

        return cargoModel;
    }

    public bool Equals(CargoModel other)
    {
        return Type.Equals(other.Type)
            && Weight.Equals(other.Weight)
            && CurrencyManager.Equals(other.CurrencyManager);
    }
}
