using System;
using System.Collections.Generic;

public class TrainMaster : Master<Train>
{
    public TrainMaster() => Collection = new();

    public Train Init(
        string name,
        TrainType type,
        TrainAttribute attribute,
        CargoHelper cargoHelper)
    {
        return new(name, type, attribute, cargoHelper);
    }

    public void AddTrain(Train train) => Add(train.Guid, train);
    public void RemoveTrain(Guid guid) => Remove(guid);
    public HashSet<Guid> GetAllTrain() => GetAll();
    public void AddCargo(Guid train, Guid cargo) => Get(train).AddCargo(cargo);
    public void RemoveCargo(Guid train, Guid cargo) => Get(train).RemoveCargo(cargo);
    public void RemoveCargoRange(Guid train, HashSet<Guid> cargos)
    {
        Get(train).RemoveCargoRange(cargos);
    }
    public HashSet<Guid> GetAllCargo(Guid train) => Get(train).GetAllCargo();
    public Guid GetDestination(Guid train) => Get(train).GetDestination();
    public void SetTravelPlan(Guid train, Guid sourceStation, Guid destinationStation)
    {
        Get(train).SetTravelPlan(sourceStation, destinationStation);
    }
}
