using System;
using System.Collections.Generic;

public class Train : Worker
{
    private TrainType _type;

    public override Enum Type { get => _type; protected set => _type = (TrainType)value; }
    public TrainAttribute Attribute { get; private set; }
    private TravelPlan TravelPlan { get; set; }
    private CargoHelper CargoHelper { get; set; }
    

    public Train(string name, TrainType type, TrainAttribute attribute, CargoHelper cargoHelper)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Type = type;
        Attribute = attribute;
        CargoHelper = cargoHelper;
    }

    public void AddCargo(Guid cargo) => CargoHelper.Add(cargo);
    public void RemoveCargo(Guid cargo) => CargoHelper.Remove(cargo);
    public void RemoveCargoRange(HashSet<Guid> cargos) => CargoHelper.RemoveRange(cargos);
    public HashSet<Guid> GetAllCargo() => CargoHelper.GetAll();
    public void SetTravelPlan(Guid sourceStation, Guid destinationStation)
    {
        TravelPlan.SetSourceStation(sourceStation);
        TravelPlan.SetDestinationStation(destinationStation);
    }
    public Guid GetDestination() => TravelPlan.DestinationStation;

    public override object Clone()
    {
        Train train = (Train)this.MemberwiseClone();

        // TODO: Need to add deep copy for Attribute

        train.CargoHelper = (CargoHelper)train.CargoHelper.Clone();
        return train;
    }
}
