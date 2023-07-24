using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Train : Worker, IEquatable<Train>
{
    private TrainType _type;

    public override Enum Type { get => _type; protected set => _type = (TrainType)value; }
    public TrainStatus Status { get; private set; }
    public TrainAttribute Attribute { get; private set; }
    public TravelPlan TravelPlan { get; private set; }
    public HashSet<Guid> CargoHelper { get; private set; }

    [JsonConstructor]
    private Train(
        string guid,
        string type,
        TrainAttribute attribute,
        TravelPlan travelPlan,
        HashSet<Guid> cargoHelper)
    {
        Guid = new(guid);
        Type = Enum.Parse<TrainType>(type);
        Attribute = attribute;
        TravelPlan = travelPlan;
        CargoHelper = cargoHelper;
    }

    public Train(TrainModel trainModel)
    {
        Guid = Guid.NewGuid();
        Type = trainModel.Type;
        Attribute = trainModel.Attribute;
        CargoHelper = new();
        TravelPlan = default;
    }

    public void Activate() => Status = TrainStatus.Active;

    public void Deactivate() => Status = TrainStatus.Inactive;

    public void FileTravelPlan(Guid sourceStation, Guid destinationStation)
    {
        TravelPlan = new(sourceStation, destinationStation);
    }

    public void CompleteTravelPlan() => TravelPlan = default;

    public override object Clone()
    {
        Train train = (Train)MemberwiseClone();

        train.Attribute = (TrainAttribute)train.Attribute.Clone();

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
                && CargoHelper.SetEquals(other.CargoHelper);
        }
        return Type.Equals(other.Type)
            && Attribute.Equals(other.Attribute)
            && TravelPlan.Equals(other.TravelPlan)
            && CargoHelper.SetEquals(other.CargoHelper);
    }
}
