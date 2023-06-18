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
    public void AddCargo(Guid trainGuid, Guid cargoGuid) => Get(trainGuid).AddCargo(cargoGuid);
    public void RemoveCargo(Guid trainGuid, Guid cargoGuid) => Get(trainGuid).RemoveCargo(cargoGuid);
    public void RemoveCargoRange(Guid trainGuid, HashSet<Guid> cargoGuids)
    {
        Get(trainGuid).RemoveCargoRange(cargoGuids);
    }
    public HashSet<Guid> GetAllCargo(Guid trainGuid) => Get(trainGuid).GetAllCargo();
    public Guid GetDestination(Guid trainGuid) => Get(trainGuid).GetDestination();
}
