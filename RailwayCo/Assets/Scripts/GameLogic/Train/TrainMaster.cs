using System;
using System.Collections.Generic;
using UnityEngine;

public class TrainMaster : IPlayfab
{
    private Dictionary<Guid, Train> Collection { get; set; }
    private Dictionary<Guid, TrainModel> Catalog { get; set; }

    public TrainMaster()
    {
        Collection = new();
        Catalog = new();

        InitCatalog();
    }

    #region Collection Management
    public Guid AddObject(
        string trainName,
        TrainType trainType,
        double maxSpeed,
        Vector3 position,
        Quaternion rotation,
        MovementDirection movementDirection,
        MovementState movementState)
    {
        TrainModel trainModel = GetTrainModel(trainType);
        trainModel.InitUnityStats(maxSpeed, position, rotation, movementDirection, movementState);
        Train train = new(trainModel);
        train.Name = trainName;

        Collection.Add(train);
        return train.Guid;
    }
    public void RemoveObject(Guid train) => Collection.Remove(train);
    public HashSet<Guid> GetAllGuids() => Collection.GetAllGuids();
    public Train GetObject(Vector3 position)
    {
        Train train = default;
        HashSet<Guid> trains = Collection.GetAllGuids();
        foreach (var guid in trains)
        {
            Train trainObject = Collection.GetRef(guid);
            if (trainObject.Attribute.Position == position)
            {
                train = trainObject;
                break;
            }
        }
        return train;
    }
    public Train GetObject(Guid train) => Collection.GetRef(train);
    #endregion

    #region Catalog Management
    private void InitCatalog()
    {
        TrainType[] trainTypes = (TrainType[])Enum.GetValues(typeof(TrainType));
        foreach (var trainType in trainTypes)
        {
            TrainAttribute trainAttribute = trainType switch
            {
                TrainType.Steam => new(new(0, 5, 0, 0),
                                       new(0, 10000, 10000, 100),
                                       new(0, 100, 100, 5),
                                       new(0, 4, 0, 0),
                                       new(),
                                       new(),
                                       MovementDirection.North,
                                       MovementState.Stationary),
                TrainType.Diesel => new(new(0, 5, 0, 0),
                                        new(0, 7500, 7500, 50),
                                        new(0, 500, 500, 5),
                                        new(0, 6, 0, 0),
                                        new(),
                                        new(),
                                        MovementDirection.North,
                                        MovementState.Stationary),
                TrainType.Electric => new(new(0, 5, 0, 0),
                                          new(0, 15000, 15000, 25),
                                          new(0, 1000, 1000, 2),
                                          new(0, 10, 0, 0),
                                          new(),
                                          new(),
                                          MovementDirection.North,
                                          MovementState.Stationary),
                _ => throw new NotImplementedException()
            };
            TrainModel trainModel = new(trainType, trainAttribute);
            Catalog.Add(trainModel);
        }
    }
    private TrainModel GetTrainModel(TrainType trainType)
    {
        HashSet<Guid> trainModels = Catalog.GetAllGuids();
        foreach (Guid trainModel in trainModels)
        {
            TrainModel trainModelObject = Catalog.GetRef(trainModel);
            if (trainType == (TrainType)trainModelObject.Type)
                return trainModelObject;
        }
        throw new InvalidProgramException("Unknown TrainType in Catalog");
    }
    #endregion

    #region Status Management
    public void ActivateTrain(Guid train)
    {
        Train trainObject = Collection.GetObject(train);
        trainObject.Activate();
    }
    public void DeactivateTrain(Guid train)
    {
        Train trainObject = Collection.GetObject(train);
        trainObject.Deactivate();
    }
    #endregion

    #region TrainAttribute Management
    public void SetUnityStats(
        Guid train,
        float speed,
        Vector3 position,
        Quaternion rotation,
        MovementDirection movementDirection,
        MovementState movementState)
    {
        Train trainObject = Collection.GetObject(train);
        trainObject.Attribute.SetUnityStats(speed, position, rotation, movementDirection, movementState);
    }
    public TrainAttribute GetTrainAttribute(Guid train)
    {
        Train trainObject = Collection.GetObject(train);
        return (TrainAttribute)trainObject.Attribute.Clone();
    }
    public void Refuel(Guid train)
    {
        Train trainObject = Collection.GetObject(train);
        trainObject.Attribute.Refuel();
    }
    public bool BurnFuel(Guid train, int fuelToBurn)
    {
        Train trainObject = Collection.GetObject(train);
        return trainObject.Attribute.BurnFuel(fuelToBurn);
    }
    public void DurabilityRepair(Guid train)
    {
        Train trainObject = Collection.GetObject(train);
        trainObject.Attribute.DurabilityRepair();
    }
    public bool DurabilityWear(Guid train)
    {
        Train trainObject = Collection.GetObject(train);
        return trainObject.Attribute.DurabilityWear();
    }
    #endregion

    #region TravelPlan Management
    public void FileTravelPlan(Guid train, Guid sourceStation, Guid destinationStation)
    {
        Train trainObject = Collection.GetObject(train);
        trainObject.FileTravelPlan(sourceStation, destinationStation);
    }
    public void CompleteTravelPlan(Guid train)
    {
        Train trainObject = Collection.GetObject(train);
        trainObject.CompleteTravelPlan();
    }
    public Guid GetDepartureStation(Guid train)
    {
        Train trainObject = Collection.GetObject(train);
        TravelPlan travelPlan = trainObject.TravelPlan;
        return travelPlan == default ? default : trainObject.TravelPlan.SourceStation;
    }
    public Guid GetArrivalStation(Guid train)
    {
        Train trainObject = Collection.GetObject(train);
        TravelPlan travelPlan = trainObject.TravelPlan;
        return travelPlan == default ? default : trainObject.TravelPlan.DestinationStation;
    }
    #endregion

    #region Cargo Management
    public bool AddCargoToTrain(Guid train, Guid cargo)
    {
        Train trainObject = Collection.GetObject(train);
        if (trainObject.Attribute.IsCapacityFull()) return false;

        trainObject.CargoHelper.Add(cargo);
        trainObject.Attribute.AddToCapacity();
        return true;
    }
    public void RemoveCargoFromTrain(Guid train, Guid cargo)
    {
        Train trainObject = Collection.GetObject(train);
        trainObject.CargoHelper.Remove(cargo);
        trainObject.Attribute.RemoveFromCapacity();
    }
    public HashSet<Guid> GetCargoManifest(Guid train)
    {
        Train trainObject = Collection.GetObject(train);
        return new(trainObject.CargoHelper);
    }
    #endregion

    #region PlayFab Management
    public string SendDataToPlayfab() => GameDataManager.Serialize(Collection);
    public void SetDataFromPlayfab(string data)
    {
        Collection = GameDataManager.Deserialize<Dictionary<Guid, Train>>(data);
    }
    #endregion
}
