using PlayFab;
using UnityEngine;

[CreateAssetMenu(fileName = "GamelogicSO", menuName = "ScriptableObjects/Gamelogic")]
public class GameLogic : ScriptableObject
{
    public User User { get; private set; }
    public CargoMaster CargoMaster { get; private set; }
    public TrainMaster TrainMaster { get; private set; }
    public StationMaster StationMaster { get; private set; }
    public PlayfabManager PlayfabManager { get; private set; }

    public void InitGameData()
    {
        // PlayfabManager.GameDataManager.GetUserData();
        // TODO: Initialize the various members
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
