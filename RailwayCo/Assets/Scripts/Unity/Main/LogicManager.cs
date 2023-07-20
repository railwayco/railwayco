using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Intermediary between all the GameObjects and Backend GameManeger/GameLogic
public class LogicManager : MonoBehaviour
{
    [SerializeField] private GameLogic _gameLogic;
    private Coroutine _sendDataToPlayfabCoroutine;

    private void Awake()
    {
        if (!_gameLogic) Debug.LogError("Game Logic is not attached to the logic manager!");
        _sendDataToPlayfabCoroutine = StartCoroutine(SendDataToPlayfabRoutine(60f));
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

    // Either Retrieve old train GUID or create a new GUID
    // TODO: Once the ability to add new trains by the user is supported, the initial load should only load existing trains from the DB
    public Guid SetupGetTrainGUID(TrainMovement trainMovScript, GameObject trainGO)
    {
        DepartDirection movementDirn = trainMovScript.MovementDirn;
        Vector3 trainPosition = trainMovScript.transform.position;
        Quaternion trainRotation = trainMovScript.transform.rotation;
        float maxSpeed = trainMovScript.MaxSpeed;

        TrainType trainType = TrainType.Steam; // TODO: determine from gameobject

        Vector3 position = trainGO.transform.position;
        Train train = GetTrainClassObject(position);
        if (train == null)
        {
            return _gameLogic.AddTrainObject(trainType, maxSpeed, trainPosition, trainRotation, movementDirn);
        }
        else
        {
            return train.Guid;
        }
    }

    //////////////////////////////////////////////////////
    /// TRAIN RELATED
    //////////////////////////////////////////////////////

    public Train GetTrainClassObject(Vector3 position)
    {
        return _gameLogic.GetTrainObject(position);
    }

    public void UpdateTrainBackend(TrainMovement trainMovScript, Guid trainGuid)
    {
        float trainCurrentSpeed = trainMovScript.CurrentSpeed;
        DepartDirection movementDirn = trainMovScript.MovementDirn;
        Vector3 trainPosition = trainMovScript.transform.position;
        Quaternion trainRotation = trainMovScript.transform.rotation;

        _gameLogic.SetTrainUnityStats(trainGuid, trainCurrentSpeed, trainPosition, trainRotation, movementDirn);
    }

    public void ReplenishTrainFuelAndDurability(Guid trainGuid)
    {
        _gameLogic.ReplenishTrainFuelAndDurability(trainGuid);
    }

    //////////////////////////////////////////////////////
    /// STATION RELATED
    //////////////////////////////////////////////////////
    public DepartStatus SetStationAsDestination(Guid trainGUID, int currentStationNum, int destinationStationNum)
    {
        Guid currentStationGUID = _gameLogic.GetStationObject(currentStationNum).Guid;
        Guid destinationStationGUID = _gameLogic.GetStationObject(destinationStationNum).Guid;
        _gameLogic.SetTrainTravelPlan(trainGUID, currentStationGUID, destinationStationGUID);
        return _gameLogic.OnTrainDeparture(trainGUID);
    }

    public Station GetIndividualStation(Guid stationGUID)
    {
        return _gameLogic.GetStationObject(stationGUID);
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
        HashSet<Guid> cargoHashset = trainRef.CargoHelper.GetAll();
        return GetCargoListFromGUIDs(cargoHashset);
    }

    // Gets either the Yard Cargo or the station cargo
    public List<Cargo> GetSelectedStationCargoList(Guid stationGUID, bool getStationCargo)
    {
        HashSet<Guid> cargoHashset;
        if (getStationCargo)
            cargoHashset = _gameLogic.GetStationCargoManifest(stationGUID);
        else
            cargoHashset = _gameLogic.GetYardCargoManifest(stationGUID);

        if (getStationCargo && cargoHashset.Count == 0) { 
            // Generate a new set of Cargo if that station is empty
            _gameLogic.AddRandomCargoToStation(stationGUID, 10);
            cargoHashset = _gameLogic.GetStationCargoManifest(stationGUID);
        }

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

    public void ProcessCargoOnTrainStop(Guid trainGUID)
    {
        _gameLogic.OnTrainArrival(trainGUID);
        UpdateBottomUIStatsPanel();
    }

    public bool ShiftCargoOnButtonClick(GameObject cargoDetailButtonGO, Cargo cargo, Guid currentTrainGUID, Guid currentStationGUID)
    {
        CargoAssociation cargoAssoc = cargo.CargoAssoc;
        if (cargoAssoc == CargoAssociation.Station || cargoAssoc == CargoAssociation.Yard)
        {
            if (!_gameLogic.AddCargoToTrain(currentTrainGUID, cargo.Guid))
                return false;

            _gameLogic.RemoveCargoFromStation(currentStationGUID, cargo.Guid);
            Destroy(cargoDetailButtonGO);
            return true;
        }
        else if (cargoAssoc == CargoAssociation.Train)
        {
            if (!_gameLogic.AddCargoToStation(currentStationGUID, cargo.Guid))
                return false;

            _gameLogic.RemoveCargoFromTrain(currentTrainGUID, cargo.Guid);
            Destroy(cargoDetailButtonGO);
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
        return _gameLogic.RemoveUserCurrencyManager(cost);
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
