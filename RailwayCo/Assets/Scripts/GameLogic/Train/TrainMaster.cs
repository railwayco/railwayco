using System;

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
}
