using System;
using Newtonsoft.Json;

public class Station : Worker, IEquatable<Station>
{
    // TODO: Have different types of Station
    public override Enum Type { get; protected set; }
    public int Number { get; private set; }
    public StationAttribute Attribute { get; private set; }
    public HashsetHelper StationHelper { get; private set; }
    public HashsetHelper TrainHelper { get; private set; }
    public HashsetHelper StationCargoHelper { get; private set; }
    public HashsetHelper YardCargoHelper { get; private set; }

    [JsonConstructor]
    private Station(
        string guid,
        int number,
        StationAttribute attribute,
        HashsetHelper stationHelper,
        HashsetHelper trainHelper,
        HashsetHelper stationCargoHelper,
        HashsetHelper yardCargoHelper)
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
        HashsetHelper stationHelper,
        HashsetHelper trainHelper,
        HashsetHelper stationCargoHelper,
        HashsetHelper yardCargoHelper)
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
        station.StationHelper = (HashsetHelper)station.StationHelper.Clone();
        station.TrainHelper = (HashsetHelper)station.TrainHelper.Clone();
        station.StationCargoHelper = (HashsetHelper)station.StationCargoHelper.Clone();
        station.YardCargoHelper = (HashsetHelper)station.YardCargoHelper.Clone();

        return station;
    }

    public bool Equals(Station other)
    {
        return Number.Equals(other.Number)
            && Attribute.Equals(other.Attribute)
            && StationHelper.Equals(other.StationHelper)
            && TrainHelper.Equals(other.TrainHelper)
            && StationCargoHelper.Equals(other.StationCargoHelper)
            && YardCargoHelper.Equals(other.YardCargoHelper);
    }
}
