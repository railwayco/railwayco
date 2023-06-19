using System;

public class TrainModel : Model
{
    private TrainType _type;

    public override Enum Type { get => _type; protected set => _type = (TrainType)value; }
    public TrainAttribute Attribute { get; private set; }

    public TrainModel(TrainType type, TrainAttribute attribute)
    {
        Guid = Guid.NewGuid();
        Type = type;
        Attribute = attribute;
    }

    // TODO: For Crate rewards. Generate TrainAttribute.

    public override object Clone()
    {
        CargoModel cargoModel = (CargoModel)this.MemberwiseClone();

        // TODO: Perform deep copy

        return cargoModel;
    }
}
