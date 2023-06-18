using System;

public class TrainCatalog : Catalog
{
    private TrainType _type;

    public override Enum Type { get => _type; protected set => _type = (TrainType)value; }
    public TrainAttribute Attribute { get; private set; }

    public TrainCatalog(TrainType type, TrainAttribute attribute)
    {
        Guid = Guid.NewGuid();
        Type = type;
        Attribute = attribute;
    }

    // TODO: For Crate rewards. Generate TrainAttribute.
}
