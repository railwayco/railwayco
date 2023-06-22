using System;
using System.Collections.Generic;

public class Train : Worker
{
    private TrainType _type;

    public override Enum Type { get => _type; protected set => _type = (TrainType)value; }
    public TrainAttribute Attribute { get; private set; }
    public TravelPlan TravelPlan { get; private set; }
    public HashsetHelper CargoHelper { get; private set; }
    
    public Train(Guid guid, string name, TrainType type, TrainAttribute attribute, HashsetHelper cargoHelper)
    {
        Guid = guid;
        Name = name;
        Type = type;
        Attribute = attribute;
        CargoHelper = cargoHelper;
    }

    public override object Clone()
    {
        Train train = (Train)this.MemberwiseClone();

        // TODO: Need to add deep copy for Attribute

        train.CargoHelper = (HashsetHelper)train.CargoHelper.Clone();
        return train;
    }
}
