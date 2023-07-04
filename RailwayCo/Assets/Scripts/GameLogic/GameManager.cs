using PlayFab;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RailwayCoSO", menuName = "ScriptableObjects/RailwayCo")]
public class GameManager : ScriptableObject
{
    public GameLogic GameLogic { get; private set; }
    public AuthManager AuthManager { get; private set; }
    public GameDataManager GameDataManager { get; private set; }

    private void OnEnable()
    {
#if UNITY_EDITOR
        // Source: https://forum.unity.com/threads/solved-but-unhappy-scriptableobject-awake-never-execute.488468/#post-5564170
        // use platform dependent compilation so it only exists in editor, otherwise it'll break the build
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            Init();
#endif       
    }

    private void Awake() => Init();

    private void Init()
    {
        PlayFabSettings.TitleId = "357DE";
        AuthManager = new();
        GameDataManager = new();
        GameLogic = new();

        GameLogic.UpdateHandler += GameLogic_UpdateHandler;
    }

    private void GameLogic_UpdateHandler(object sender, Dictionary<GameDataType, string> gameDataDict)
    {
        if (gameDataDict.Count == 0) return;
        GameDataManager.UpdateUserData(gameDataDict);
    }
}
