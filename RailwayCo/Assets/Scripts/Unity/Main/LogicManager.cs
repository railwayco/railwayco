using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Intermediary between all the GameObjects and Backend GameLogic
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

    /// <summary>Based on the platform, try to retrieve existing station GUID.</summary>
    public Guid GetStationGuid(string platformName)
    {
        Tuple<int, int> stationPlatformTuple = GetStationPlatformNumbers(platformName);
        int stationNum = stationPlatformTuple.Item1;        
        Station station = _gameLogic.GetStationObject(stationNum);

        if (station is null)
            return _gameLogic.AddStationObject(stationNum);
        return station.Guid;
    }

    /// <summary>Retrieve platform GUID</summary>
    public Guid GetPlatformGuid(string platformName) => GetPlatformGUID(platformName);

    //////////////////////////////////////////////////////
    /// TRAIN RELATED
    //////////////////////////////////////////////////////

    

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

    public bool MoveCargoBetweenTrainAndStation(Cargo cargo, Guid trainGuid, Guid stationGuid)
    {
        CargoAssociation cargoAssoc = cargo.CargoAssoc;
        if (cargoAssoc == CargoAssociation.Station || cargoAssoc == CargoAssociation.Yard)
        {
            if (!_gameLogic.AddCargoToTrain(trainGuid, cargo.Guid))
                return false;
            _gameLogic.RemoveCargoFromStation(stationGuid, cargo.Guid);
        }
        else if (cargoAssoc == CargoAssociation.Train)
        {
            if (!_gameLogic.AddCargoToStation(stationGuid, cargo.Guid))
                return false;
            _gameLogic.RemoveCargoFromTrain(trainGuid, cargo.Guid);
        }
        else
        {
            Debug.LogError($"There is currently no logic being implemented for CargoAssociation {cargoAssoc}");
            return false;
        }
        return true;
    }


    //////////////////////////////////////////////////////
    /// UNLOCKING RELATED
    //////////////////////////////////////////////////////
    
    public bool UnlockTracks(string trackSectionName, CurrencyManager currMgr)
    {
        string[] platforms = trackSectionName.Split('-');

        Guid src = GetPlatformGUID(platforms[0]);
        Guid dst = GetPlatformGUID(platforms[1]);
        if (!_gameLogic.UnlockTrack(src, dst, currMgr))
            return false;
        UserManager.UpdateUserStatsPanel();
        return true;
    }

    public bool UnlockPlatform(Guid platform, CurrencyManager currMgr)
    {
        if (!_gameLogic.UnlockPlatform(platform, currMgr))
            return false;
        UserManager.UpdateUserStatsPanel();
        return true;
    }

    public bool AbleToPurchase(CurrencyManager cost)
    {
        if (_gameLogic.RemoveUserCurrencyManager(cost))
        {
            UserManager.UpdateUserStatsPanel();
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
}
