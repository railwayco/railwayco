using System.Collections.Generic;

public class TrainManager
{
    private Dictionary<string, Train> trainDict;

    private Dictionary<string, Train> TrainDict { get => trainDict; set => trainDict = value; }
    public List<string> TrainList => new(TrainDict.Keys);

    public TrainManager() => TrainDict = new();
    public void AddTrain(Train train) => TrainDict.Add(train.TrainName, train);
    public bool RemoveTrain(Train train) => TrainDict.Remove(train.TrainName);
}
