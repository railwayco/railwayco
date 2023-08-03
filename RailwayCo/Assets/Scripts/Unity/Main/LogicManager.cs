using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Intermediary between all the GameObjects and Backend GameLogic
public class LogicManager : MonoBehaviour
{
    [SerializeField] private GameLogic _gameLogic;
    private Coroutine _sendDataToPlayfabCoroutine;

    // TODO: To be removed when train prefab manager is added
    [SerializeField] private GameObject _trainPrefab;
    private GameObject _trainList;

    private void Awake()
    {
        if (!_gameLogic) Debug.LogError("Game Logic is not attached to the logic manager!");
        _sendDataToPlayfabCoroutine = StartCoroutine(SendDataToPlayfabRoutine(60f));

        _trainList = GameObject.Find("TrainList");
        if (!_trainList) Debug.LogError("Train List not found");
    }

    private void Start()
    {
        UpdateBottomUIStatsPanel();
        SetupAllTrains();
    }

    //////////////////////////////////////////////////////
    /// PLAYFAB RELATED
    //////////////////////////////////////////////////////

    private IEnumerator SendDataToPlayfabRoutine(float secondsTimeout)
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsTimeout);
            _gameLogic.SendDataToPlayfab();
        }

        // TODO: Graceful termination when signalled by
        // OnApplicationPause or OnApplicationQuit
        // that will be implemented using StopCoroutine
    }


    //////////////////////////////////////////////////////
    /// SETUP RELATED
    //////////////////////////////////////////////////////

    // Based on the platform, try to retrieve existing station GUID.
    public Guid SetupGetStationGUID(GameObject platformGO)
    {
        Tuple<int, int> stationPlatformTuple = GetStationPlatformNumbers(platformGO.name);
        int stationNum = stationPlatformTuple.Item1;        
        Station station = _gameLogic.GetStationObject(stationNum);

        if (station is null)
        {
            return _gameLogic.AddStationObject(stationNum);
        }
        else
        {
            return station.Guid;
        }
    }

    // Retrieve platform GUID
    public Guid SetupGetPlatformGUID(GameObject platformGO)
    {
        return GetPlatformGUID(platformGO.name);
    }

    private void SetupAllTrains()
    {
        HashSet<Guid> trainGuids = _gameLogic.GetAllTrainGuids();

        // Default no train then setup first train in platform 1_1
        if (trainGuids.Count == 0)
        {
            // Get Platform 1_1 position
            Vector3 deltaVertical = new Vector3(0, -0.53f, -1);
            GameObject platform1_1 = GameObject.Find("Platform1_1");
            if (!platform1_1)
            {
                Debug.LogError("Platform 1_1 is not found!");
                return;
            }
            Vector3 platformPos = platform1_1.transform.position;

            // string lineName = platform1_1.GetComponent<PlatformManager>().GetLineName();
            string lineName = "LineA";
            string trainName = $"{lineName}_Train";
            TrainType trainType = TrainType.Steam;
            Vector3 trainPosition = platformPos + deltaVertical;
            Quaternion trainRotation = Quaternion.identity;

            Guid trainGuid = AddTrainToBackend(trainName, trainType, trainPosition, trainRotation);
            InitNewTrainInScene(trainGuid);
            return;
        }

        trainGuids = _gameLogic.GetAllTrainGuids();
        foreach (Guid trainGuid in trainGuids)
        {
            InitNewTrainInScene(trainGuid);
        }
    }

    //////////////////////////////////////////////////////
    /// TRAIN RELATED
    //////////////////////////////////////////////////////

    public void InitNewTrainInScene(Guid trainGuid)
    {
        string trainName = GetTrainName(trainGuid);
        TrainAttribute trainAttribute = GetTrainAttribute(trainGuid);
        Vector3 position = trainAttribute.Position;
        Quaternion rotation = trainAttribute.Rotation;
        GameObject train = Instantiate(_trainPrefab, position, rotation, _trainList.transform);
        train.name = trainName;
    }

    public Guid AddTrainToBackend(string trainName, TrainType trainType, Vector3 position, Quaternion rotation)
    {
        double maxSpeed = 10;
        MovementDirection movementDirn = MovementDirection.West;
        MovementState movement = MovementState.Stationary;
        Guid trainGuid = _gameLogic.AddTrainObject(trainName, trainType, maxSpeed, position, rotation, movementDirn, movement);
        return trainGuid;
    }
    
    public Train GetTrainClassObject(Vector3 position)
    {
        return _gameLogic.GetTrainObject(position);
    }

    public TrainAttribute GetTrainAttribute(Guid trainGuid)
    {
        return _gameLogic.GetTrainAttribute(trainGuid);
    }

    public string GetTrainName(Guid trainGuid)
    {
        Train train = _gameLogic.GetTrainObject(trainGuid);
        return train != default ? train.Name : "";
    }

    public void UpdateTrainBackend(TrainAttribute trainAttribute, Guid trainGuid)
    {
        float trainCurrentSpeed = (float)trainAttribute.Speed.Amount;
        Vector3 trainPosition = trainAttribute.Position;
        Quaternion trainRotation = trainAttribute.Rotation;
        MovementDirection movementDirn = trainAttribute.MovementDirection;
        MovementState movementState = trainAttribute.MovementState;

        _gameLogic.SetTrainUnityStats(trainGuid, trainCurrentSpeed, trainPosition, trainRotation, movementDirn, movementState);
    }

    public void RefuelTrain(Guid trainGuid)
    {
        _gameLogic.TrainRefuel(trainGuid);
    }

    public bool RepairTrain(Guid train, CurrencyManager cost)
    {
        bool result = _gameLogic.SpeedUpTrainRepair(train, cost);
        if (result)
            UpdateBottomUIStatsPanel();
        return result;
    }

    public void OnTrainCollision(Guid trainGuid)
    {
        _gameLogic.OnTrainCollision(trainGuid);
    }

    //////////////////////////////////////////////////////
    /// STATION RELATED
    //////////////////////////////////////////////////////
    public DepartStatus SetStationAsDestination(Guid trainGUID, int srcStationNum, int destStationNum, int fuelToBurn)
    {
        Guid currentStationGUID = GetStationGuidFromStationNum(srcStationNum);
        Guid destinationStationGUID = GetStationGuidFromStationNum(destStationNum);
        _gameLogic.SetTrainTravelPlan(trainGUID, currentStationGUID, destinationStationGUID);
        return _gameLogic.OnTrainDeparture(trainGUID, fuelToBurn);
    }

    public Station GetIndividualStation(Guid stationGUID)
    {
        return _gameLogic.GetStationObject(stationGUID);
    }

    public Guid GetStationGuidFromStationNum(int stationNum)
    {
        return _gameLogic.GetStationObject(stationNum).Guid;
    }

    public StationAttribute GetStationAttribute(Guid stationGuid)
    {
        return GetIndividualStation(stationGuid).Attribute;
    }

    //////////////////////////////////////////////////////
    /// PLATFORM RELATED
    //////////////////////////////////////////////////////

    public Guid GetPlatformGUID(string platformName)
    {
        Tuple<int, int> stationPlatformTuple = GetStationPlatformNumbers(platformName);
        int stationNum = stationPlatformTuple.Item1;
        int platformNum = stationPlatformTuple.Item2;
        return _gameLogic.GetPlatformGuid(stationNum, platformNum);
    }

    public OperationalStatus GetTrackStatus(string trackName)
    {
        string[] platforms = trackName.Split('-');
        if (platforms.Length != 2)
        {
            Debug.LogError("Issue with parsing track name");
            return OperationalStatus.Locked;
        }
        string platform1 = platforms[0];
        Guid platform1GUID = GetPlatformGUID(platform1);

        string platform2 = platforms[1];
        Guid platform2GUID = GetPlatformGUID(platform2);

        return _gameLogic.GetTrackStatus(platform1GUID, platform2GUID);
    }

    public OperationalStatus GetPlatformStatus(Guid platformGUID)
    {
        return _gameLogic.GetPlatformStatus(platformGUID);
    }

    //////////////////////////////////////////////////////
    /// CARGO LIST RETRIEVAL
    //////////////////////////////////////////////////////

    public List<Cargo> GetTrainCargoList(Guid trainGUID)
    {
        if (trainGUID == Guid.Empty)
        {
            Debug.LogError("Invalid trainGUID to get associated cargo");
            return null;
        }

        Train trainRef = _gameLogic.GetTrainObject(trainGUID);
        HashSet<Guid> cargoHashset = trainRef.CargoHelper;
        return GetCargoListFromGUIDs(cargoHashset);
    }

    public List<Cargo> GetStationCargoList(Guid stationGUID)
    {
        HashSet<Guid> cargoHashset = _gameLogic.GetStationCargoManifest(stationGUID);
        return GetCargoListFromGUIDs(cargoHashset);
    }

    public List<Cargo> GetYardCargoList(Guid stationGUID)
    {
        HashSet<Guid> cargoHashset = _gameLogic.GetYardCargoManifest(stationGUID);
        return GetCargoListFromGUIDs(cargoHashset);
    }

    private List<Cargo> GetCargoListFromGUIDs(HashSet<Guid> cargoHashset)
    {
        List<Cargo> cargoList = new();
        foreach (Guid guid in cargoHashset)
        {
            cargoList.Add(_gameLogic.GetCargoObject(guid));
        }
        return cargoList;
    }

    //////////////////////////////////////////////////////
    /// CARGO PROCESSING AND SHIFTING
    //////////////////////////////////////////////////////

    public void ProcessCargoOnTrainStop(Guid trainGUID, Guid stationGuid)
    {
        if (_gameLogic.GetTrainDepartureStation(trainGUID) == default)
            _gameLogic.OnTrainRestoration(trainGUID, stationGuid);
        else
        {
            _gameLogic.OnTrainArrival(trainGUID);
            UpdateBottomUIStatsPanel();
        }
    }

    public bool MoveCargoBetweenTrainAndStation(Cargo cargo, Guid trainGuid, Guid stationGuid)
    {
        CargoAssociation cargoAssoc = cargo.CargoAssoc;
        if (cargoAssoc == CargoAssociation.Station || cargoAssoc == CargoAssociation.Yard)
        {
            if (!_gameLogic.AddCargoToTrain(trainGuid, cargo.Guid))
                return false;

            _gameLogic.RemoveCargoFromStation(stationGuid, cargo.Guid);
            return true;
        }
        else if (cargoAssoc == CargoAssociation.Train)
        {
            if (!_gameLogic.AddCargoToStation(stationGuid, cargo.Guid))
                return false;

            _gameLogic.RemoveCargoFromTrain(trainGuid, cargo.Guid);
            return true;
        }
        else
        {
            Debug.LogError($"There is currently no logic being implemented for CargoAssociation {cargoAssoc}");
        }
        return false;
    }


    //////////////////////////////////////////////////////
    /// UNLOCKING RELATED
    //////////////////////////////////////////////////////
    
    public bool UnlockTracks(string trackSectionName, CurrencyManager currMgr)
    {
        string[] platforms = trackSectionName.Split('-');

        Guid src = GetPlatformGUID(platforms[0]);
        Guid dst = GetPlatformGUID(platforms[1]);
        if (!_gameLogic.UnlockTrack(src,dst, currMgr))
            return false;
        UpdateBottomUIStatsPanel();
        return true;
    }

    public bool UnlockPlatform(string platformName, CurrencyManager currMgr)
    {
        Guid platform = GetPlatformGUID(platformName);
        if (!_gameLogic.UnlockPlatform(platform, currMgr))
            return false;
        UpdateBottomUIStatsPanel();
        return true;
    }

    public bool AbleToPurchase(CurrencyManager cost)
    {
        if (_gameLogic.RemoveUserCurrencyManager(cost))
        {
            UpdateBottomUIStatsPanel();
            return true;
        }
        return false;
    }

    //////////////////////////////////////////////////////
    /// Additional Utility Methods
    //////////////////////////////////////////////////////

    public static Tuple<int, int> GetStationPlatformNumbers(string platformName)
    {
        string copyName = platformName.Replace("Platform", "");
        string[] numStrArray = copyName.Split('_');
        if (numStrArray.Length != 2)
        {
            Debug.LogError("Issue with parsing platform name");
            return default;
        }
        return new(int.Parse(numStrArray[0]), int.Parse(numStrArray[1]));
    }

    public void UpdateBottomUIStatsPanel()
    {
        int exp = _gameLogic.GetUserExperiencePoints();
        CurrencyManager currMgr = GetUserCurrencyStats();       
        BottomPanelManager bpm = GameObject.Find("MainUI").transform.Find("BottomPanel").GetComponent<BottomPanelManager>();
        bpm.SetUIStatsInformation(currMgr, exp);
    }

    public CurrencyManager GetUserCurrencyStats()
    {
        return _gameLogic.GetUserCurrencyManager();
    }
}
