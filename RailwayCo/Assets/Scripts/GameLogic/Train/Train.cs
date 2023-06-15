public class Train
{
    private string trainName;
    private TrainType trainType;
    private TrainAttribute trainAttribute;
    private CargoManager cargoManager;

    public string TrainName { get => trainName; set => trainName = value; }
    public TrainType TrainType { get => trainType; private set => trainType = value; }
    public TrainAttribute TrainAttribute { get => trainAttribute; private set => trainAttribute = value; }
    public CargoManager CargoManager { get => cargoManager; private set => cargoManager = value; }

    public Train(string trainName, TrainType trainType, TrainAttribute trainAttribute, CargoManager cargoManager)
    {
        TrainName = trainName;
        TrainType = trainType;
        TrainAttribute = trainAttribute;
        CargoManager = cargoManager;
    }
}
