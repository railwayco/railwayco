using System;
using Newtonsoft.Json;

public class Station : Worker
{
    private StationStatus status;

    public override Enum Type { get => status; protected set => status = (StationStatus)value; }
    public StationAttribute Attribute { get; private set; }
    public DictHelper<StationOrientation> StationHelper { get; private set; }
    public HashsetHelper TrainHelper { get; private set; }
    public HashsetHelper CargoHelper { get; private set; }
    

    [JsonConstructor]
    private Station(
        string guid,
        string name,
        string type,
        StationAttribute attribute,
        DictHelper<StationOrientation> stationHelper,
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
        DictHelper<StationOrientation> stationHelper,
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
        station.StationHelper = (DictHelper<StationOrientation>)station.StationHelper.Clone();
        station.TrainHelper = (HashsetHelper)station.TrainHelper.Clone();
        station.CargoHelper = (HashsetHelper)station.CargoHelper.Clone();

        return station;
    }
}
