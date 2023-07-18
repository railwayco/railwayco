using System;
using Newtonsoft.Json;

public class CargoModel : Worker, IEquatable<CargoModel>
{
    private CargoType _type;

    public override Enum Type { get => _type; protected set => _type = (CargoType)value; }
    public IntAttribute Weight { get; private set; }
    public RangedCurrencyManager RangedCurrencyManager { get; private set; }

    [JsonConstructor]
    private CargoModel(
        string guid,
        string type,
        IntAttribute weight,
        RangedCurrencyManager rangedCurrencyManager)
    {
        Guid = new(guid);
        Type = Enum.Parse<CargoType>(type);
        Weight = weight;
        RangedCurrencyManager = rangedCurrencyManager;
    }

    public CargoModel(
        CargoType type,
        int weightLowerLimit,
        int weightUpperLimit,
        RangedCurrencyManager rangedCurrencyManager)
    {
        Guid = Guid.NewGuid();
        Type = type;
        Weight = new(weightLowerLimit, weightUpperLimit, 0, 0);
        RangedCurrencyManager = rangedCurrencyManager;
    }

    public void Randomise()
    {
        Random rand = new();
        int lowerLimit = Weight.LowerLimit;
        int upperLimit = Weight.UpperLimit;
        Weight.Amount = rand.Next(lowerLimit, upperLimit + 1);
        RangedCurrencyManager.Randomise();
    }

    public override object Clone()
    {
        CargoModel cargoModel = (CargoModel)MemberwiseClone();

        cargoModel.Weight = (IntAttribute)cargoModel.Weight.Clone();
        cargoModel.RangedCurrencyManager = (RangedCurrencyManager)cargoModel.RangedCurrencyManager.Clone();

        return cargoModel;
    }

    public bool Equals(CargoModel other)
    {
        return Type.Equals(other.Type)
            && Weight.Equals(other.Weight)
            && RangedCurrencyManager.Equals(other.RangedCurrencyManager);
    }
}
