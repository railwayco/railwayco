using System;
using Newtonsoft.Json;
using UnityEngine;

public class Train : Worker
{
    private TrainType _type;

    public override Enum Type { get => _type; protected set => _type = (TrainType)value; }
    public TrainAttribute Attribute { get; private set; }
    public TravelPlan TravelPlan { get; private set; }
    public HashsetHelper CargoHelper { get; private set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public TrainDirection Direction { get; private set; }
    
    [JsonConstructor]
    private Train(
        string guid,
        string name,
        string type,
        TrainAttribute attribute,
        HashsetHelper cargoHelper,
        Vector3 position,
        Quaternion rotation,
        string direction)
    {
        Guid = new(guid);
        Name = name;
        Type = Enum.Parse<TrainType>(type);
        Attribute = attribute;
        CargoHelper = cargoHelper;
        Position = position;
        Rotation = rotation;
        Direction = Enum.Parse<TrainDirection>(direction);
    }

    public Train(
        string name,
        TrainType type,
        TrainAttribute attribute,
        HashsetHelper cargoHelper,
        Vector3 position,
        Quaternion rotation,
        TrainDirection direction)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Type = type;
        Attribute = attribute;
        CargoHelper = cargoHelper;
        TravelPlan = new TravelPlan(Guid.Empty, Guid.Empty);
        Position = position;
        Rotation = rotation;
        Direction = direction;
    }

    public override object Clone()
    {
        Train train = (Train)this.MemberwiseClone();

        // TODO: Need to add deep copy for Attribute

        train.CargoHelper = (HashsetHelper)train.CargoHelper.Clone();
        return train;
    }
}
