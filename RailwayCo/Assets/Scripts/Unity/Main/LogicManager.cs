using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Intermediary between all the GameObjects and Backend GameManeger/GameLogic
public class LogicManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    private Coroutine _sendDataToPlayfabCoroutine;

    private void Awake()
    {
        if (!_gameManager) Debug.LogError("Game Manager is not attached to the logic manager!");
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
            _gameManager.GameLogic.SendDataToPlayfab();
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
        Station station = _gameManager.GameLogic.GetStationObject(stationNum);

        if (station is null)
        {
            return _gameManager.GameLogic.AddStationObject(stationNum);
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

        Vector3 position = trainGO.transform.position;
        Train train = GetTrainClassObject(position);
        if (train == null)
        {
            return _gameManager.GameLogic.AddTrainObject(trainGO.name, maxSpeed, trainPosition, trainRotation, movementDirn);
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
        return _gameManager.GameLogic.GetTrainObject(position);
    }

    public void UpdateTrainBackend(TrainMovement trainMovScript, Guid trainGuid)
    {
        float trainCurrentSpeed = trainMovScript.CurrentSpeed;
        DepartDirection movementDirn = trainMovScript.MovementDirn;
        Vector3 trainPosition = trainMovScript.transform.position;
        Quaternion trainRotation = trainMovScript.transform.rotation;

        _gameManager.GameLogic.SetTrainUnityStats(trainGuid,
                                                  trainCurrentSpeed,
                                                  trainPosition,
                                                  trainRotation,
                                                  movementDirn);
    }

    public void ReplenishTrainFuelAndDurability(Guid trainGuid)
    {
        _gameManager.GameLogic.ReplenishTrainFuelAndDurability(trainGuid);
    }

    //////////////////////////////////////////////////////
    /// STATION RELATED
    //////////////////////////////////////////////////////
    public DepartStatus SetStationAsDestination(Guid trainGUID, int currentStationNum, int destinationStationNum)
    {
        Guid currentStationGUID = _gameManager.GameLogic.GetStationObject(currentStationNum).Guid;
        Guid destinationStationGUID = _gameManager.GameLogic.GetStationObject(destinationStationNum).Guid;
        _gameManager.GameLogic.SetTrainTravelPlan(trainGUID, currentStationGUID, destinationStationGUID);
        return _gameManager.GameLogic.OnTrainDeparture(trainGUID);
    }

    public Station GetIndividualStation(Guid stationGUID)
    {
        return _gameManager.GameLogic.GetStationObject(stationGUID);
    }

    //////////////////////////////////////////////////////
    /// PLATFORM RELATED
    //////////////////////////////////////////////////////

    public Guid GetPlatformGUID(string platformName)
    {
        Tuple<int, int> stationPlatformTuple = GetStationPlatformNumbers(platformName);
        int stationNum = stationPlatformTuple.Item1;
        int platformNum = stationPlatformTuple.Item2;
        return _gameManager.GameLogic.GetPlatformGuid(stationNum, platformNum);
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

        return _gameManager.GameLogic.GetTrackStatus(platform1GUID, platform2GUID);
    }

    public OperationalStatus GetPlatformStatus(Guid platformGUID)
    {
        return _gameManager.GameLogic.GetPlatformStatus(platformGUID);
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

        Train trainRef = _gameManager.GameLogic.GetTrainObject(trainGUID);
        HashSet<Guid> cargoHashset = trainRef.CargoHelper.GetAll();
        return GetCargoListFromGUIDs(cargoHashset);
    }

    // Gets either the Yard Cargo or the station cargo
    public List<Cargo> GetSelectedStationCargoList(Guid stationGUID, bool getStationCargo)
    {
        // Gets all the station AND yard cargo, since they are under the same cargoHelper in the station
        HashSet<Guid> cargoHashset = _gameManager.GameLogic.GetStationObject(stationGUID).CargoHelper.GetAll();

        if (cargoHashset.Count == 0) { 
            // Generate a new set of Cargo if that station is empty
            _gameManager.GameLogic.AddRandomCargoToStation(stationGUID, 10);
            cargoHashset = _gameManager.GameLogic.GetStationObject(stationGUID).CargoHelper.GetAll();
        }

        List<Cargo> allStationCargo = GetCargoListFromGUIDs(cargoHashset);
        return GetStationSubCargo(allStationCargo, getStationCargo);
    }

    /// By default, the call to get the station cargo returns both (new) station cargo and also yard cargo
    /// This functions serves to return the sub-category of the cargo
    /// Returns Either the station cargo or the yard cargo
    private List<Cargo> GetStationSubCargo(List<Cargo> allStationCargo, bool getStation)
    {
        List<Cargo> output = new List<Cargo>();
        foreach (Cargo cargo in allStationCargo)
        {
            CargoAssociation cargoAssoc = cargo.CargoAssoc;
            if (getStation && cargoAssoc == CargoAssociation.Station) // Get Station-Only cargo
            {
                output.Add(cargo);
            }
            else if (!getStation && cargoAssoc == CargoAssociation.Yard)// Get Yard-Only cargo
            {
                output.Add(cargo);
            }
            else continue;
        }
        return output;
    }

    private List<Cargo> GetCargoListFromGUIDs(HashSet<Guid> cargoHashset)
    {
        List<Cargo> cargoList = new();
        foreach (Guid guid in cargoHashset)
        {
            cargoList.Add(_gameManager.GameLogic.GetCargoObject(guid));
        }
        return cargoList;
    }

    //////////////////////////////////////////////////////
    /// CARGO PROCESSING AND SHIFTING
    //////////////////////////////////////////////////////

    public void ProcessCargoOnTrainStop(Guid trainGUID)
    {
        _gameManager.GameLogic.OnTrainArrival(trainGUID);
        UpdateBottomUIStatsPanel();
    }

    public bool ShiftCargoOnButtonClick(GameObject cargoDetailButtonGO, Cargo cargo, Guid currentTrainGUID, Guid currentStationGUID)
    {
        CargoAssociation cargoAssoc = cargo.CargoAssoc;
        if (cargoAssoc == CargoAssociation.Station || cargoAssoc == CargoAssociation.Yard)
        {
            if (!_gameManager.GameLogic.AddCargoToTrain(currentTrainGUID, cargo.Guid))
                return false;

            _gameManager.GameLogic.RemoveCargoFromStation(currentStationGUID, cargo.Guid);
            Destroy(cargoDetailButtonGO);
            return true;
        }
        else if (cargoAssoc == CargoAssociation.Train)
        {
            if (!_gameManager.GameLogic.AddCargoToStation(currentStationGUID, cargo.Guid))
                return false;

            _gameManager.GameLogic.RemoveCargoFromTrain(currentTrainGUID, cargo.Guid);
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
    
    public void UnlockTracks(string trackSectionName, CurrencyManager currMgr)
    {
        string[] platforms = trackSectionName.Split('-');

        Guid src = GetPlatformGUID(platforms[0]);
        Guid dst = GetPlatformGUID(platforms[1]);
        _gameManager.GameLogic.UnlockNewTrack(src,dst);
        _gameManager.GameLogic.User.RemoveCurrencyViaCurrencyManager(currMgr);
        UpdateBottomUIStatsPanel();
    }

    public void UnlockPlatform(string platformName, CurrencyManager currMgr)
    {
        Guid platform = GetPlatformGUID(platformName);
        _gameManager.GameLogic.UnlockNewPlatform(platform);
        _gameManager.GameLogic.User.RemoveCurrencyViaCurrencyManager(currMgr);
        UpdateBottomUIStatsPanel();
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
        int exp = _gameManager.GameLogic.GetUserExperiencePoints();
        CurrencyManager currMgr = GetUserCurrencyStats();        
        BottomPanelManager bpm = GameObject.Find("MainUI").transform.Find("BottomPanel").GetComponent<BottomPanelManager>();
        bpm.SetUIStatsInformation(currMgr, exp);
    }

    public CurrencyManager GetUserCurrencyStats()
    {
        return _gameManager.GameLogic.GetUserCurrencyManager();
    }
}
