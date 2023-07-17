using System;
using Newtonsoft.Json;
using UnityEngine;

public class TrainModel : Worker
{
    private TrainType _type;

    public override Enum Type { get => _type; protected set => _type = (TrainType)value; }
    public TrainAttribute Attribute { get; private set; }

    [JsonConstructor]
    private TrainModel(string guid, string type, TrainAttribute attribute)
    {
        Guid = new(guid);
        Type = Enum.Parse<TrainType>(type);
        Attribute = attribute;
    }

    public TrainModel(TrainType type, TrainAttribute attribute)
    {
        Guid = Guid.NewGuid();
        Type = type;
        Attribute = attribute;
    }

    public void InitUnityStats(double maxSpeed, Vector3 position, Quaternion rotation, DepartDirection direction)
    {
        DoubleAttribute speed = Attribute.Speed; 
        Attribute = new(
            Attribute.Capacity,
            Attribute.Fuel,
            Attribute.Durability,
            new(speed.LowerLimit, maxSpeed, speed.Amount, speed.Rate),
            position,
            rotation,
            direction);
    }

    public override object Clone()
    {
        TrainModel trainModel = (TrainModel)MemberwiseClone();

        trainModel.Attribute = (TrainAttribute)trainModel.Attribute.Clone();

        return trainModel;
    }
}
