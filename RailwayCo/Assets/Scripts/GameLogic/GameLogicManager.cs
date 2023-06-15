using PlayFab;
using UnityEngine;

[CreateAssetMenu(fileName = "GamelogicScriptableObject", menuName = "ScriptableObjects/Gamelogic")]
public class GameLogicManager : ScriptableObject
{
    private User user;
    private StationMaster stationMaster;
    private PlayfabManager playfabManager;

    public User User { get => user; private set => user = value; }
    public StationMaster StationMaster { get => stationMaster; private set => stationMaster = value; }
    public PlayfabManager PlayfabManager { get => playfabManager; private set => playfabManager = value; }

    public void InitGameData(User user, StationMaster stationMaster)
    {
        User = user;
        StationMaster = stationMaster;
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        // Source: https://forum.unity.com/threads/solved-but-unhappy-scriptableobject-awake-never-execute.488468/#post-5564170
        // use platform dependent compilation so it only exists in editor, otherwise it'll break the build
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            Init();
#endif       
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        PlayFabSettings.TitleId = "357DE";
        PlayfabManager = new();
    }
}
