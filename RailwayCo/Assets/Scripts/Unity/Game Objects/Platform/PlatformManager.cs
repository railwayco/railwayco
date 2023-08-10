using System;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    private static PlatformManager Instance { get; set; }
    private static Dictionary<Guid, GameObject> GameObjectDict { get; set; }

    [SerializeField] private GameLogic _gameLogic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
            GameObjectDict = new();
        }

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
    /// GAMEOBJECTDICT METHODS
    //////////////////////////////////////////////////////

    public static void RegisterPlatform(Guid platformGuid, GameObject gameObject)
    {
        GameObjectDict.Add(platformGuid, gameObject);
    }

    public static GameObject GetGameObject(Guid platformGuid) => GameObjectDict.GetValueOrDefault(platformGuid);

    public static Guid GetPlatformGuid(GameObject platform)
    {
        PlatformController platformCtr = platform.GetComponent<PlatformController>();
        if (!platformCtr) return default;
        return platformCtr.PlatformGuid;
    }

    public static Guid GetStationGuid(GameObject platform)
    {
        PlatformController platformCtr = platform.GetComponent<PlatformController>();
        if (!platformCtr) return default;
        return platformCtr.StationGuid;
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
