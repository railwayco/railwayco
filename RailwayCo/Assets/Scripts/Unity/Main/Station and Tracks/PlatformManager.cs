using System;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    private static PlatformManager Instance { get; set; }

    [SerializeField] private GameLogic _gameLogic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        if (!Instance._gameLogic) Debug.LogError("Game Logic is not attached to the Platform Manager");
    }

    /// <summary>Based on the platform, try to retrieve existing station GUID.</summary>
    public static Guid GetStationGuid(string platformName)
    {
        Tuple<int, int> stationPlatformTuple = GetStationPlatformNumbers(platformName);
        int stationNum = stationPlatformTuple.Item1;
        Station station = Instance._gameLogic.GetStationObject(stationNum);

        if (station is null)
            return Instance._gameLogic.AddStationObject(stationNum);
        return station.Guid;
    }

    /// <summary>Retrieve platform GUID</summary>
    public static Guid GetPlatformGuid(string platformName)
    {
        Tuple<int, int> stationPlatformTuple = GetStationPlatformNumbers(platformName);
        int stationNum = stationPlatformTuple.Item1;
        int platformNum = stationPlatformTuple.Item2;
        return Instance._gameLogic.GetPlatformGuid(stationNum, platformNum);
    }

    //////////////////////////////////////////////////////
    /// STATION RELATED
    //////////////////////////////////////////////////////
    public static DepartStatus SetStationAsDestination(Guid trainGUID, int srcStationNum, int destStationNum, int fuelToBurn)
    {
        Guid currentStationGUID = GetStationGuidFromStationNum(srcStationNum);
        Guid destinationStationGUID = GetStationGuidFromStationNum(destStationNum);
        Instance._gameLogic.SetTrainTravelPlan(trainGUID, currentStationGUID, destinationStationGUID);
        return Instance._gameLogic.OnTrainDeparture(trainGUID, fuelToBurn);
    }

    public static Station GetStationClassObject(Guid stationGUID)
    {
        return Instance._gameLogic.GetStationObject(stationGUID);
    }

    public static Guid GetStationGuidFromStationNum(int stationNum)
    {
        return Instance._gameLogic.GetStationObject(stationNum).Guid;
    }

    public static StationAttribute GetStationAttribute(Guid stationGuid)
    {
        return GetStationClassObject(stationGuid).Attribute;
    }

    //////////////////////////////////////////////////////
    /// PLATFORM RELATED
    //////////////////////////////////////////////////////

    public static OperationalStatus GetPlatformStatus(Guid platformGUID)
    {
        return Instance._gameLogic.GetPlatformStatus(platformGUID);
    }

    public static bool UnlockPlatform(Guid platform, CurrencyManager currMgr)
    {
        if (!Instance._gameLogic.UnlockPlatform(platform, currMgr))
            return false;
        UserManager.UpdateUserStatsPanel();
        return true;
    }

    //////////////////////////////////////////////////////
    /// UTILITY METHODS
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
