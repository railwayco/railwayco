using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Station : Worker, IEquatable<Station>
{
    // TODO: Have different types of Station
    public override Enum Type { get; protected set; }
    public int Number { get; private set; }
    public StationAttribute Attribute { get; private set; }
    public HashSet<Guid> StationHelper { get; private set; }
    public HashSet<Guid> TrainHelper { get; private set; }
    public HashSet<Guid> StationCargoHelper { get; private set; }
    public HashSet<Guid> YardCargoHelper { get; private set; }

    [JsonConstructor]
    private Station(
        string guid,
        int number,
        StationAttribute attribute,
        HashSet<Guid> stationHelper,
        HashSet<Guid> trainHelper,
        HashSet<Guid> stationCargoHelper,
        HashSet<Guid> yardCargoHelper)
    {
        Guid = new(guid);
        Number = number;
        Attribute = attribute;
        StationHelper = stationHelper;
        TrainHelper = trainHelper;
        StationCargoHelper = stationCargoHelper;
        YardCargoHelper = yardCargoHelper;
    }

    public Station(
        int number,
        StationAttribute stationAttribute,
        HashSet<Guid> stationHelper,
        HashSet<Guid> trainHelper,
        HashSet<Guid> stationCargoHelper,
        HashSet<Guid> yardCargoHelper)
    {
        Guid = Guid.NewGuid();
        Number = number;
        Attribute = stationAttribute;
        StationHelper = stationHelper;
        TrainHelper = trainHelper;
        StationCargoHelper = stationCargoHelper;
        YardCargoHelper = yardCargoHelper;
    }

    public override object Clone()
    {
        Station station = (Station)MemberwiseClone();

        station.Attribute = (StationAttribute)station.Attribute.Clone();

        return station;
    }

    public bool Equals(Station other)
    {
        return Number.Equals(other.Number)
            && Attribute.Equals(other.Attribute)
            && StationHelper.SetEquals(other.StationHelper)
            && TrainHelper.SetEquals(other.TrainHelper)
            && StationCargoHelper.SetEquals(other.StationCargoHelper)
            && YardCargoHelper.SetEquals(other.YardCargoHelper);
    }
}
