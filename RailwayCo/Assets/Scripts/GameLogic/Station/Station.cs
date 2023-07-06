using System;
using Newtonsoft.Json;

public class Station : Worker, IEquatable<Station>
{
    private StationStatus status;

    public override Enum Type { get => status; protected set => status = (StationStatus)value; }
    public StationAttribute Attribute { get; private set; }
    public HashsetHelper StationHelper { get; private set; }
    public HashsetHelper TrainHelper { get; private set; }
    public HashsetHelper CargoHelper { get; private set; }
    

    [JsonConstructor]
    private Station(
        string guid,
        string name,
        string type,
        StationAttribute attribute,
        HashsetHelper stationHelper,
        HashsetHelper trainHelper,
        HashsetHelper cargoHelper)
    {
        Guid = new(guid);
        Name = name;
        Type = Enum.Parse<StationStatus>(type);
        Attribute = attribute;
        StationHelper = stationHelper;
        TrainHelper = trainHelper;
        CargoHelper = cargoHelper;
    }

    public Station(
        string name,
        StationStatus status,
        StationAttribute stationAttribute,
        HashsetHelper stationHelper,
        HashsetHelper trainHelper,
        HashsetHelper cargoHelper)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Type = status;
        Attribute = stationAttribute;
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
        Station station = (Station)MemberwiseClone();

        station.Attribute = (StationAttribute)station.Attribute.Clone();
        station.StationHelper = (HashsetHelper)station.StationHelper.Clone();
        station.TrainHelper = (HashsetHelper)station.TrainHelper.Clone();
        station.CargoHelper = (HashsetHelper)station.CargoHelper.Clone();

        return station;
    }

    public bool Equals(Station other)
    {
        return Type.Equals(other.Type)
            && Attribute.Equals(other.Attribute)
            && StationHelper.Equals(other.StationHelper)
            && TrainHelper.Equals(other.TrainHelper)
            && CargoHelper.Equals(other.CargoHelper);
    }
}
