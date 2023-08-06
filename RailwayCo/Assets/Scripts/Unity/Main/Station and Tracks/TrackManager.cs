using System;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    private static TrackManager Instance { get; set; }

    [SerializeField] private GameLogic _gameLogic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        if (!Instance._gameLogic) Debug.LogError("Game Logic is not attached to the Track Manager");
    }

    public static bool UnlockTracks(string trackSectionName, CurrencyManager currMgr)
    {
        string[] platforms = trackSectionName.Split('-');

        Guid src = PlatformManager.GetPlatformGuid(platforms[0]);
        Guid dst = PlatformManager.GetPlatformGuid(platforms[1]);
        if (!Instance._gameLogic.UnlockTrack(src, dst, currMgr))
            return false;
        UserManager.UpdateUserStatsPanel();
        return true;
    }

    public static OperationalStatus GetTrackStatus(string trackName)
    {
        string[] platforms = trackName.Split('-');
        if (platforms.Length != 2)
        {
            Debug.LogError("Issue with parsing track name");
            return OperationalStatus.Locked;
        }
        string platform1 = platforms[0];
        Guid platform1GUID = PlatformManager.GetPlatformGuid(platform1);

        string platform2 = platforms[1];
        Guid platform2GUID = PlatformManager.GetPlatformGuid(platform2);

        return Instance._gameLogic.GetTrackStatus(platform1GUID, platform2GUID);
    }
}
