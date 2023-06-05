using System.Collections.Generic;

public class TrainManager
{
    private Dictionary<string, Train> trainDict;

    private Dictionary<string, Train> TrainDict { get => trainDict; set => trainDict = value; }

    public TrainManager()
    {
        TrainDict = new();
    }

    public void AddTrain(Train train)
    {
        TrainDict.Add(train.TrainName, train);
    }

    public bool RemoveTrain(Train train)
    {
        return TrainDict.Remove(train.TrainName);
    }

    public List<string> GetTrainList()
    {
        return new List<string>(TrainDict.Keys);
    }
}
