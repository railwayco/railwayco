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

    public void AddCargo(Guid cargoGuid) => CargoHelper.Add(cargoGuid);
    public void RemoveCargo(Guid cargoGuid) => CargoHelper.Remove(cargoGuid);
    public void RemoveCargoRange(HashSet<Guid> cargoGuids) => CargoHelper.RemoveRange(cargoGuids);
    public HashSet<Guid> GetAllCargo() => CargoHelper.GetAll();
    public void SetTravelPlan(Guid sourceStationGuid, Guid destinationStationGuid)
    {
        TravelPlan.SetSourceStation(sourceStationGuid);
        TravelPlan.SetDestinationStation(destinationStationGuid);
    }
    public Guid GetDestination() => TravelPlan.DestinationStationGuid;
}
