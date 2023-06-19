using System;

public class CargoModel : Model
{
    private CargoType _type;

    public override Enum Type { get => _type; protected set => _type = (CargoType)value; }
    public Attribute<double> Weight { get; private set; }
    public CurrencyManager CurrencyManager { get; private set; }

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
        CargoModel cargoModel = (CargoModel)this.MemberwiseClone();
        
        Attribute<double> weight = cargoModel.Weight;
        cargoModel.Weight = new(weight.LowerLimit, weight.UpperLimit, double.NaN, 0);
        
        CurrencyManager currencyManager = new();
        currencyManager.AddCurrencyManager(cargoModel.CurrencyManager);
        cargoModel.CurrencyManager = currencyManager;
        
        return cargoModel;
    }
}
