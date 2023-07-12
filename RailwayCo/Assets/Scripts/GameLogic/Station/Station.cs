using System;
using Newtonsoft.Json;

public class Station : Worker, IEquatable<Station>
{
    // TODO: Have different types of Station
    public override Enum Type { get; protected set; }
    public OperationalStatus Status { get; private set; }
    public StationAttribute Attribute { get; private set; }
    public HashsetHelper TrainHelper { get; private set; }
    public HashsetHelper CargoHelper { get; private set; }

    [JsonConstructor]
    private Station(
        string guid,
        string name,
        string status,
        StationAttribute attribute,
        HashsetHelper trainHelper,
        HashsetHelper cargoHelper)
    {
        Guid = new(guid);
        Name = name;
        Status = Enum.Parse<OperationalStatus>(status);
        Attribute = attribute;
        TrainHelper = trainHelper;
        CargoHelper = cargoHelper;
    }

    public Station(
        string name,
        OperationalStatus status,
        StationAttribute stationAttribute,
        HashsetHelper trainHelper,
        HashsetHelper cargoHelper)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Status = status;
        Attribute = stationAttribute;
        TrainHelper = trainHelper;
        CargoHelper = cargoHelper;
    }

    public void Open() => Status = OperationalStatus.Open;
    public void Close() => Status = OperationalStatus.Closed;
    public void Lock() => Status = OperationalStatus.Locked;
    public void Unlock() => Open();

    public override object Clone()
    {
        Station station = (Station)MemberwiseClone();

        station.Attribute = (StationAttribute)station.Attribute.Clone();
        station.TrainHelper = (HashsetHelper)station.TrainHelper.Clone();
        station.CargoHelper = (HashsetHelper)station.CargoHelper.Clone();

        return station;
    }

    public bool Equals(Station other)
    {
        return Status.Equals(other.Status)
            && Attribute.Equals(other.Attribute)
            && TrainHelper.Equals(other.TrainHelper)
            && CargoHelper.Equals(other.CargoHelper);
    }
}
