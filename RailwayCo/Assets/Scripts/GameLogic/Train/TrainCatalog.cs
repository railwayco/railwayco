using System;

public class TrainCatalog : Catalog<TrainModel>
{
    public TrainCatalog() => Collection = new();

    public TrainModel Init(TrainType type, TrainAttribute attribute)
    {
        return new(type, attribute);
    }

    public void AddTrainModel(TrainModel trainModel) => Add(trainModel.Guid, trainModel);
    public void RemoveTrainModel(Guid guid) => Remove(guid);
}
