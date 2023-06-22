using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class GameDataManager
{
    public event EventHandler<string> SuccessHandler;
    public event EventHandler<string> ErrorHandler;
    public event EventHandler<Dictionary<string, UserDataRecord>> DataHandler;

    public JsonSerializer Serializer { get; private set; }

    public GameDataManager()
    {
        Serializer = new();
        Serializer.Converters.Add(new StringEnumConverter());
    }

    enum GameDataEventType
    {
        GetUserData,
        UpdateUserData,
        DeleteUserData
    }

    public void GetUserData(List<GameDataType> gameDataTypes)
    {
        GameDataEventType eventType = GameDataEventType.GetUserData;

        List<string> dataTypeStringList = new();
        foreach(var gameDataType in gameDataTypes)
        {
            dataTypeStringList.Add(gameDataType.ToString());
        }

        var request = new GetUserDataRequest
        {
            Keys = dataTypeStringList
        };
        PlayFabClientAPI.GetUserData(
            request,
            (result) => OnSuccess(eventType, result),
            (playFabError) => OnError(eventType, playFabError));
    }

    public void UpdateUserData(Dictionary<GameDataType, object> gameDataTypes)
    {
        GameDataEventType eventType = GameDataEventType.UpdateUserData;

        Dictionary<string, string> dataDictionary = new();

        foreach (var gameData in gameDataTypes)
        {
            using StringWriter strWriter = new();
            Serializer.Serialize(strWriter, gameData.Value);

            string serializedKey = gameData.Key.ToString();
            string serializedValue = strWriter.GetStringBuilder().ToString();

            dataDictionary[serializedKey] = serializedValue;
        }

        var request = new UpdateUserDataRequest
        {
            Data = dataDictionary
        };
        PlayFabClientAPI.UpdateUserData(
            request,
            (result) => OnSuccess(eventType, result),
            (playFabError) => OnError(eventType, playFabError));
    }

    public void DeleteUserData(List<GameDataType> gameDataTypes)
    {
        GameDataEventType eventType = GameDataEventType.DeleteUserData;

        List<string> dataTypeStringList = new();
        gameDataTypes.ForEach(gameDataType => dataTypeStringList.Add(gameDataType.ToString()));

        var request = new UpdateUserDataRequest
        {
            KeysToRemove = dataTypeStringList
        };
        PlayFabClientAPI.UpdateUserData(
            request,
            (result) => OnSuccess(eventType, result),
            (playFabError) => OnError(eventType, playFabError));
    }

    public void DeleteAllUserData()
    {
        List<GameDataType> gameDataTypes = new((GameDataType[])Enum.GetValues(typeof(GameDataType)));
        DeleteUserData(gameDataTypes);
    }

    private void OnSuccess(GameDataEventType eventType, object result)
    {
        string dataEvent = eventType.ToString();
        SuccessHandler?.Invoke(this, dataEvent);

        Debug.Log(dataEvent + " successful");
        if (eventType is GameDataEventType.GetUserData)
        {
            Dictionary<string, UserDataRecord> data;
            data = ((GetUserDataResult)result).Data;
            DataHandler?.Invoke(this, data);
        }
    }

    private void OnError(GameDataEventType eventType, PlayFabError playFabError)
    {
        string errorMsg = playFabError.GenerateErrorReport();
        ErrorHandler?.Invoke(this, errorMsg);

        switch (eventType)
        {
            case GameDataEventType.GetUserData:
                {
                    Debug.LogError("Error while getting user data");
                    break;
                }
            case GameDataEventType.UpdateUserData:
                {
                    Debug.LogError("Error while updating user data");
                    break;
                }
            case GameDataEventType.DeleteUserData:
                {
                    Debug.LogError("Error while deleting user data");
                    break;
                }
        }
        Debug.Log(errorMsg);
    }
}
