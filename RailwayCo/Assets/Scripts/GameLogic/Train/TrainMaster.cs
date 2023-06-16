using System;

public class TrainMaster : Master<Train>
{
    public TrainMaster() => Collection = new();

    public Train Init() => new();
    public void AddTrain(Train train) => Add(train.Guid, train);
    public void RemoveTrain(Guid guid) => Remove(guid);
}
