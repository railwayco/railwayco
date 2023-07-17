using System;
using Newtonsoft.Json;

public class Train : Worker, IEquatable<Train>
{
    private TrainType _type;

    public override Enum Type { get => _type; protected set => _type = (TrainType)value; }
    public TrainAttribute Attribute { get; private set; }
    public TravelPlan TravelPlan { get; private set; }
    public HashsetHelper CargoHelper { get; private set; }

    [JsonConstructor]
    private Train(
        string guid,
        string name,
        string type,
        TrainAttribute attribute,
        TravelPlan travelPlan,
        HashsetHelper cargoHelper)
    {
        Guid = new(guid);
        Name = name;
        Type = Enum.Parse<TrainType>(type);
        Attribute = attribute;
        TravelPlan = travelPlan;
        CargoHelper = cargoHelper;
    }

    public Train(
        string name,
        TrainType type,
        TrainAttribute attribute,
        HashsetHelper cargoHelper)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Type = type;
        Attribute = attribute;
        CargoHelper = cargoHelper;
        TravelPlan = default;
    }

    public Train(TrainModel trainModel)
    {
        Guid = Guid.NewGuid();
        Type = trainModel.Type;
        Attribute = trainModel.Attribute;
        CargoHelper = new();
        TravelPlan = default;
    }

    public void FileTravelPlan(Guid sourceStation, Guid destinationStation)
    {
        TravelPlan = new(sourceStation, destinationStation);
    }

    public void CompleteTravelPlan() => TravelPlan = default;

    public override object Clone()
    {
        Train train = (Train)MemberwiseClone();

        train.Attribute = (TrainAttribute)train.Attribute.Clone();
        train.CargoHelper = (HashsetHelper)train.CargoHelper.Clone();

        return train;
    }

    public bool Equals(Train other)
    {
        if (TravelPlan == default)
        {
            if (other.TravelPlan != default)
                return false;
            return Type.Equals(other.Type)
                && Attribute.Equals(other.Attribute)
                && CargoHelper.Equals(other.CargoHelper);
        }
        return Type.Equals(other.Type)
            && Attribute.Equals(other.Attribute)
            && TravelPlan.Equals(other.TravelPlan)
            && CargoHelper.Equals(other.CargoHelper);
    }
}
