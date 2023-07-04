using System;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Slider slider;
    [SerializeField] private SceneChanger sceneChanger;

    private int progress;
    private int total;

    void Start()
    {
        List<GameDataType> gameDataTypes = new((GameDataType[])Enum.GetValues(typeof(GameDataType)));

        progress = 0;
        total = gameDataTypes.Count - 1; // TODO: Remove the -1 after using TrainCatalog
        GameDataManager.DataHandler += GameDataManager_DataHandler;

        GameDataManager.GetUserData(gameDataTypes);
    }

    private void GameDataManager_DataHandler(object sender, Dictionary<string, UserDataRecord> userData)
    {
        if (userData.Count == 0)
        {
            gameManager.GameLogic.GenerateCargoModels();
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

            gameManager.GameLogic.SetDataFromPlayfab(dataType, deserializedObject);
            progress++;
        }
        progress = total;
    }

    void Update()
    {
        slider.value = progress / total;
        if (progress == total) sceneChanger.sceneChangeEvent.Invoke(Scene.WorldMap);
    }
}
