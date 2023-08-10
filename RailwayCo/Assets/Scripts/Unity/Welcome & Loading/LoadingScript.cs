using System;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{
    [SerializeField] private GameLogic _gameLogic;
    [SerializeField] private Slider _slider;
    [SerializeField] private SceneChanger _sceneChanger;

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
        foreach (var kvp in userData)
        {
            string data = kvp.Value.Value;
            GameDataType dataType = Enum.Parse<GameDataType>(kvp.Key);
            _gameLogic.SetDataFromPlayfab(dataType, data);
            progress++;
        }
        progress = total;
    }

    void Update()
    {
        _slider.value = progress / total;
        if (progress == total) _sceneChanger.sceneChangeEvent.Invoke(Scene.MainWorld);
    }
}
