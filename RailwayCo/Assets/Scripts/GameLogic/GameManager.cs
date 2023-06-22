using System;
using System.Collections.Generic;
using System.IO;
using PlayFab;
using PlayFab.ClientModels;
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

        AuthManager.SuccessHandler += AuthManager_SuccessHandler;
        GameDataManager.DataHandler += GameDataManager_DataHandler;
    }

    private void AuthManager_SuccessHandler(object sender, string e)
    {
        List<GameDataType> gameDataTypes = new();
        gameDataTypes.Add(GameDataType.GameLogic);
        GameDataManager.GetUserData(gameDataTypes);
    }

    private void GameDataManager_DataHandler(object sender, Dictionary<string, UserDataRecord> userData)
    {
        // For now will only contain 1 key value pair of GameLogic to GameLogic data
        // TODO: Change it to handle multiple data types

        if (userData.Count == 0)
        {
            GameLogic = new();

            // TODO: Move to outside of foreach when remove GenerateRandomData
            GameLogic.UpdateHandler += GameLogic_UpdateHandler;
            Debug.Log("Generating random data now");
            GameLogic.GenerateRandomData();
            return;
        }

        foreach (var kvp in userData)
        {
            // Type dataType = Type.GetType(kvp.Key);
            string dataValue = kvp.Value.Value;

            StringReader reader = new(dataValue);
            GameLogic = (GameLogic)GameDataManager.Serializer.Deserialize(reader, typeof(GameLogic));
            GameLogic.UpdateHandler += GameLogic_UpdateHandler; // Remove this when remove GenerateRandomData
        }
    }

    private void GameLogic_UpdateHandler(object sender, GameLogic e)
    {
        Dictionary<GameDataType, object> dataToUpdate = new();
        dataToUpdate[GameDataType.GameLogic] = e;
        GameDataManager.UpdateUserData(dataToUpdate);
    }
}
