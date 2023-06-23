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
        GameLogic = new();

        AuthManager.SuccessHandler += AuthManager_SuccessHandler;
        GameDataManager.DataHandler += GameDataManager_DataHandler;
        GameLogic.UpdateHandler += GameLogic_UpdateHandler;
    }

    private void AuthManager_SuccessHandler(object sender, string e)
    {
        List<GameDataType> gameDataTypes = new((GameDataType[])Enum.GetValues(typeof(GameDataType)));
        GameDataManager.GetUserData(gameDataTypes);
    }

    private void GameDataManager_DataHandler(object sender, Dictionary<string, UserDataRecord> userData)
    {
        // For now will only contain 1 key value pair of GameLogic to GameLogic data
        // TODO: Change it to handle multiple data types

        if (userData.Count == 0)
        {
            GameLogic.GenerateRandomData();
            return;
        }

        foreach (var kvp in userData)
        {
            string data = kvp.Value.Value;
            GameDataType dataType = (GameDataType)Enum.Parse(typeof(GameDataType), kvp.Key);
            object deserializedObject = null;

            // TODO: Utilise kvp.Value.LastUpdated for synchronization feature

            switch (dataType)
            {
                case GameDataType.User:
                    {
                        deserializedObject = GameDataManager.Deserialize(typeof(User), data);
                        break;
                    }
                case GameDataType.CargoMaster:
                    {
                        deserializedObject = GameDataManager.Deserialize(typeof(WorkerDictHelper<Cargo>), data);
                        break;
                    }
                case GameDataType.CargoCatalog:
                    {
                        deserializedObject = GameDataManager.Deserialize(typeof(WorkerDictHelper<CargoModel>), data);
                        break;
                    }
                case GameDataType.TrainMaster:
                    {
                        deserializedObject = GameDataManager.Deserialize(typeof(WorkerDictHelper<Train>), data);
                        break;
                    }
                case GameDataType.TrainCatalog:
                    {
                        deserializedObject = GameDataManager.Deserialize(typeof(WorkerDictHelper<TrainModel>), data);
                        break;
                    }
                case GameDataType.StationMaster:
                    {
                        deserializedObject = GameDataManager.Deserialize(typeof(WorkerDictHelper<Station>), data);
                        break;
                    }
                case GameDataType.StationReacher:
                    {
                        deserializedObject = GameDataManager.Deserialize(typeof(StationReacher), data);
                        break;
                    }
            }

            GameLogic.SetDataFromPlayfab(dataType, deserializedObject);
        }
    }

    private void GameLogic_UpdateHandler(object sender, GameDataType dataType)
    {
        Dictionary<GameDataType, string> dataToUpdate = new();
        dataToUpdate[dataType] = GameDataManager.Serialize(sender);
        GameDataManager.UpdateUserData(dataToUpdate);
    }
}
