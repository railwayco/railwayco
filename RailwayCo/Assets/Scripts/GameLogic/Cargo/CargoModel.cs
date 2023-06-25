using System;
using Newtonsoft.Json;

public class CargoModel : Worker
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
        
        Attribute<double> weight = cargoModel.Weight;
        cargoModel.Weight = new(weight.LowerLimit, weight.UpperLimit, double.NaN, 0);
        
        CurrencyManager currencyManager = new();
        currencyManager.AddCurrencyManager(cargoModel.CurrencyManager);
        cargoModel.CurrencyManager = currencyManager;
        
        return cargoModel;
    }
}
