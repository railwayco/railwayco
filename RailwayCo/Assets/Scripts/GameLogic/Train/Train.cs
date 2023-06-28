using System;
using Newtonsoft.Json;

public class Train : Worker
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
        TravelPlan = new TravelPlan(Guid.Empty, Guid.Empty);
    }

    public override object Clone()
    {
        Train train = (Train)MemberwiseClone();

        train.Attribute = (TrainAttribute)train.Attribute.Clone();
        train.CargoHelper = (HashsetHelper)train.CargoHelper.Clone();

        return train;
    }
}
