public class Cargo
{
    private CargoType cargoType;
    private Attribute<float> weight;
    private CurrencyManager currencyManager;
    private Station sourceStation;
    private Station destinationStation;

    public CargoType CargoType { get => cargoType; private set => cargoType = value; }
    public Attribute<float> Weight { get => weight; private set => weight = value; }
    public CurrencyManager CurrencyManager { get => currencyManager; private set => currencyManager = value; }
    public Station SourceStation { get => sourceStation; private set => sourceStation = value; }
    public Station DestinationStation { get => destinationStation; private set => destinationStation = value; }

    public Cargo(
        CargoType cargoType,
        Attribute<float> weight,
        CurrencyManager currencyManager,
        Station sourceStation,
        Station destinationStation)
    {
        CargoType = cargoType;
        Weight = weight;
        CurrencyManager = currencyManager;
        SourceStation = sourceStation;
        DestinationStation = destinationStation;
    }
}
