using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Station : Worker
{
    private StationStatus status;

    public override Enum Type { get => status; protected set => status = (StationStatus)value; }
    public DictHelper<StationOrientation> StationHelper { get; private set; }
    public HashsetHelper TrainHelper { get; private set; }
    public HashsetHelper CargoHelper { get; private set; }
    public Attribute<int> YardCapacity { get; private set; }

    [JsonConstructor]
    private Station(
        string guid,
        string name,
        string type,
        HashsetHelper stationHelper,
        HashsetHelper trainHelper,
        HashsetHelper cargoHelper)
    {
        Guid = new(guid);
        Name = name;
        Type = Enum.Parse<StationStatus>(type);
        StationHelper = stationHelper;
        TrainHelper = trainHelper;
        CargoHelper = cargoHelper;
    }

    public Station(
        string name,
        StationStatus status,
        DictHelper<StationOrientation> stationHelper,
        HashsetHelper trainHelper,
        HashsetHelper cargoHelper,
        Attribute<int> yardCapacity)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Type = status;
        StationHelper = stationHelper;
        TrainHelper = trainHelper;
        CargoHelper = cargoHelper;
        YardCapacity = yardCapacity;
    }

    public void Open() => Type = StationStatus.Open;
    public void Close() => Type = StationStatus.Closed;
    public void Lock() => Type = StationStatus.Locked;
    public void Unlock() => Open();

    public bool IsYardFull() => YardCapacity.Amount == YardCapacity.UpperLimit;
    public void AddToYard() => YardCapacity.Amount++;
    public void RemoveFromYard() => YardCapacity.Amount--;
    public void UpgradeYardCapacity(int yardCapacity) => YardCapacity.UpperLimit += yardCapacity;

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
