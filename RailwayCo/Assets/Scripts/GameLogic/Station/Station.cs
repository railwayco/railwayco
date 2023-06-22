using System;
using System.Collections.Generic;

public class Station : Worker
{
    private StationStatus status;

    public override Enum Type { get => status; protected set => status = (StationStatus)value; }
    public DictHelper<StationOrientation> StationHelper { get; private set; }
    public HashsetHelper TrainHelper { get; private set; }
    public HashsetHelper CargoHelper { get; private set; }

    public Station(
        string name,
        StationStatus status,
        DictHelper<StationOrientation> stationHelper,
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
        Station station = (Station)MemberwiseClone();

        Dictionary<Guid, StationOrientation> newDict = new(station.StationHelper.Collection);
        station.StationHelper = new();
        foreach (var keyValuePair in newDict)
        {
            station.StationHelper.Add(keyValuePair.Key, keyValuePair.Value);
        }
        station.TrainHelper = (HashsetHelper)station.TrainHelper.Clone();
        station.CargoHelper = (HashsetHelper)station.CargoHelper.Clone();

        return station;
    }
}
