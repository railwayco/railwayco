using System;
using Newtonsoft.Json;

public class Station : Worker, IEquatable<Station>
{
    // TODO: Have different types of Station
    public override Enum Type { get; protected set; }
    public OperationStatus Status { get; private set; }
    public StationAttribute Attribute { get; private set; }
    public HashsetHelper StationHelper { get; private set; }
    public HashsetHelper TrainHelper { get; private set; }
    public HashsetHelper CargoHelper { get; private set; }

    [JsonConstructor]
    private Station(
        string guid,
        string name,
        string status,
        StationAttribute attribute,
        HashsetHelper stationHelper,
        HashsetHelper trainHelper,
        HashsetHelper cargoHelper)
    {
        Guid = new(guid);
        Name = name;
        Status = Enum.Parse<OperationStatus>(status);
        Attribute = attribute;
        StationHelper = stationHelper;
        TrainHelper = trainHelper;
        CargoHelper = cargoHelper;
    }

    public Station(
        string name,
        OperationStatus status,
        StationAttribute stationAttribute,
        HashsetHelper stationHelper,
        HashsetHelper trainHelper,
        HashsetHelper cargoHelper)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Status = status;
        Attribute = stationAttribute;
        StationHelper = stationHelper;
        TrainHelper = trainHelper;
        CargoHelper = cargoHelper;
    }

    public void Open() => Status = OperationStatus.Open;
    public void Close() => Status = OperationStatus.Closed;
    public void Lock() => Status = OperationStatus.Locked;
    public void Unlock() => Open();

    public override object Clone()
    {
        Station station = (Station)MemberwiseClone();

        station.Attribute = (StationAttribute)station.Attribute.Clone();
        station.StationHelper = (HashsetHelper)station.StationHelper.Clone();
        station.TrainHelper = (HashsetHelper)station.TrainHelper.Clone();
        station.CargoHelper = (HashsetHelper)station.CargoHelper.Clone();

        return station;
    }

    public bool Equals(Station other)
    {
        return Status.Equals(other.Status)
            && Attribute.Equals(other.Attribute)
            && StationHelper.Equals(other.StationHelper)
            && TrainHelper.Equals(other.TrainHelper)
            && CargoHelper.Equals(other.CargoHelper);
    }
}
