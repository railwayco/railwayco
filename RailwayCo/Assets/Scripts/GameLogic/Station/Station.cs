using System;

public class Station : Worker
{
    private StationStatus status;

    public override Enum Type { get => status; protected set => status = (StationStatus)value; }
    public HashsetHelper StationHelper { get; private set; }
    public HashsetHelper TrainHelper { get; private set; }
    public HashsetHelper CargoHelper { get; private set; }

    public Station(
        string name,
        StationStatus status,
        HashsetHelper stationHelper,
        HashsetHelper trainHelper,
        HashsetHelper cargoHelper)
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

    public override object Clone()
    {
        Station station = (Station)this.MemberwiseClone();

        station.StationHelper = (HashsetHelper)station.StationHelper.Clone();
        station.TrainHelper = (HashsetHelper)station.TrainHelper.Clone();
        station.CargoHelper = (HashsetHelper)station.CargoHelper.Clone();

        return station;
    }
}
