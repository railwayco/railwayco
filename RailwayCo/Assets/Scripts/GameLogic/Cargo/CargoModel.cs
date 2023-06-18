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

    // TODO: For random Cargo generation. Randomise weight.
}
