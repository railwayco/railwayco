using System;

public class Station : Worker
{
    private StationStatus status;

    public override Enum Type { get => status; protected set => status = (StationStatus)value; }
    public StationHelper StationHelper { get; private set; }
    public TrainHelper TrainHelper { get; private set; }
    public CargoHelper CargoHelper { get; private set; }

    public Station(
        string name,
        StationStatus status,
        StationHelper stationHelper,
        TrainHelper trainHelper,
        CargoHelper cargoHelper)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Type = status;
        StationHelper = stationHelper;
        TrainHelper = trainHelper;
        CargoHelper = cargoHelper;
    }

    public void Open() => Type = StationStatus.Open;
    public void Close() => Type = StationStatus.Closed;
    public void Lock() => Type = StationStatus.Locked;
    public void Unlock() => Open();
}
