using System;
using Newtonsoft.Json;

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

    // TODO: For Crate rewards. Generate TrainAttribute.

    public override object Clone()
    {
        CargoModel cargoModel = (CargoModel)MemberwiseClone();

        // TODO: Perform deep copy

        return cargoModel;
    }
}
