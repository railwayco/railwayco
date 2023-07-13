using System;
using Newtonsoft.Json;

public class Station : Worker, IEquatable<Station>
{
    // TODO: Have different types of Station
    public override Enum Type { get; protected set; }
    public int Number { get; private set; }
    public OperationalStatus Status { get; private set; }
    public StationAttribute Attribute { get; private set; }
    public HashsetHelper StationHelper { get; private set; }
    public HashsetHelper TrainHelper { get; private set; }
    public HashsetHelper CargoHelper { get; private set; }

    [JsonConstructor]
    private Station(
        string guid,
        int number,
        string status,
        StationAttribute attribute,
        HashsetHelper stationHelper,
        HashsetHelper trainHelper,
        HashsetHelper cargoHelper)
    {
        Guid = new(guid);
        Number = number;
        Status = Enum.Parse<OperationalStatus>(status);
        Attribute = attribute;
        StationHelper = stationHelper;
        TrainHelper = trainHelper;
        CargoHelper = cargoHelper;
    }

    public Station(
        int number,
        OperationalStatus status,
        StationAttribute stationAttribute,
        HashsetHelper stationHelper,
        HashsetHelper trainHelper,
        HashsetHelper cargoHelper)
    {
        Guid = Guid.NewGuid();
        Number = number;
        Status = status;
        Attribute = stationAttribute;
        StationHelper = stationHelper;
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
        station.StationHelper = (HashsetHelper)station.StationHelper.Clone();
        station.TrainHelper = (HashsetHelper)station.TrainHelper.Clone();
        station.CargoHelper = (HashsetHelper)station.CargoHelper.Clone();

        return station;
    }

    public bool Equals(Station other)
    {
        return Number.Equals(other.Number)
            && Status.Equals(other.Status)
            && Attribute.Equals(other.Attribute)
            && StationHelper.Equals(other.StationHelper)
            && TrainHelper.Equals(other.TrainHelper)
            && CargoHelper.Equals(other.CargoHelper);
    }
}
