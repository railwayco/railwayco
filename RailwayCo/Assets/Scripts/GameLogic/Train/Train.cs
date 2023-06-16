using System;

public class Train : Worker
{
    private TrainType _type;

    public override Enum Type { get => _type; protected set => _type = (TrainType)value; }
    public TrainAttribute Attribute { get; private set; }
    private CargoHelper CargoHelper { get; set; }
    
    public Train(string name, TrainType type, TrainAttribute attribute, CargoHelper cargoHelper)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Type = type;
        Attribute = attribute;
        CargoHelper = cargoHelper;
    }

    public void AddCargo(Cargo cargo) => CargoHelper.Add(cargo.Guid);
    public void RemoveCargo(Cargo cargo) => CargoHelper.Remove(cargo.Guid);
}
