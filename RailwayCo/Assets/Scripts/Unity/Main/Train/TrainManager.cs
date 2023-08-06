using System;
using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    private static TrainManager Instance { get; set; }

    [SerializeField] private GameLogic _gameLogic;
    [SerializeField] private GameObject _trainPrefab;

    private GameObject _trainList;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        if (!Instance._gameLogic) Debug.LogError("Game Logic is not attached to the Train Manager");

        Instance._trainList = gameObject;
    }

    private void Start()
    {
        // Setup all trains
        HashSet<Guid> trainGuids = Instance._gameLogic.GetAllTrainGuids();

        // Default no train then setup first train in platform 1_1
        if (trainGuids.Count == 0)
            SetupFirstTrain();

        trainGuids = Instance._gameLogic.GetAllTrainGuids();
        foreach (Guid trainGuid in trainGuids)
            InstantiateTrainInScene(trainGuid);
    }

    private static Guid SetupFirstTrain()
    {
        // Get Platform 1_1 position
        Vector3 deltaVertical = new(0, -0.53f, -1);
        GameObject platform1_1 = GameObject.Find("Platform1_1");
        if (!platform1_1)
        {
            Debug.LogError("Platform 1_1 is not found!");
            return default;
        }
        Vector3 platformPos = platform1_1.transform.position;

        // string lineName = platform1_1.GetComponent<PlatformManager>().GetLineName();
        string lineName = "LineA";
        string trainName = $"{lineName}_Train";
        TrainType trainType = TrainType.Steam;
        Vector3 trainPosition = platformPos + deltaVertical;
        Quaternion trainRotation = Quaternion.identity;

        return AddTrainToBackend(trainName, trainType, trainPosition, trainRotation);
    }

    public static Guid AddTrainToBackend(string trainName, TrainType trainType, Vector3 position, Quaternion rotation)
    {
        double maxSpeed = 10;
        MovementDirection movementDirn = MovementDirection.West;
        MovementState movement = MovementState.Stationary;
        Guid trainGuid = Instance._gameLogic.AddTrainObject(trainName, trainType, maxSpeed, position, rotation, movementDirn, movement);
        return trainGuid;
    }

    public static void InstantiateTrainInScene(Guid trainGuid)
    {
        string trainName = GetTrainName(trainGuid);
        TrainAttribute trainAttribute = GetTrainAttribute(trainGuid);
        Vector3 position = trainAttribute.Position;
        Quaternion rotation = trainAttribute.Rotation;
        GameObject train = Instantiate(Instance._trainPrefab, position, rotation, Instance._trainList.transform);
        train.name = trainName;
    }

    public static Train GetTrainClassObject(Vector3 position) => Instance._gameLogic.GetTrainObject(position);

    public static string GetTrainName(Guid trainGuid)
    {
        Train train = Instance._gameLogic.GetTrainObject(trainGuid);
        return train != default ? train.Name : "";
    }

    public static TrainAttribute GetTrainAttribute(Guid trainGuid) => Instance._gameLogic.GetTrainAttribute(trainGuid);

    public static void UpdateTrainBackend(TrainAttribute trainAttribute, Guid trainGuid)
    {
        float trainCurrentSpeed = (float)trainAttribute.Speed.Amount;
        Vector3 trainPosition = trainAttribute.Position;
        Quaternion trainRotation = trainAttribute.Rotation;
        MovementDirection movementDirn = trainAttribute.MovementDirection;
        MovementState movementState = trainAttribute.MovementState;

        Instance._gameLogic.SetTrainUnityStats(trainGuid, trainCurrentSpeed, trainPosition, trainRotation, movementDirn, movementState);
    }

    public static void RefuelTrain(Guid trainGuid) => Instance._gameLogic.TrainRefuel(trainGuid);

    public static bool RepairTrain(Guid trainGuid, CurrencyManager cost)
    {
        bool result = Instance._gameLogic.SpeedUpTrainRepair(trainGuid, cost);
        if (result)
            UserManager.UpdateUserStatsPanel();
        return result;
    }

    public static void OnTrainCollision(Guid trainGuid) => Instance._gameLogic.OnTrainCollision(trainGuid);
}
