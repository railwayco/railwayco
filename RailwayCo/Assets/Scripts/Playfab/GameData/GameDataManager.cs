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
    public static event EventHandler<string> SuccessHandler;
    public static event EventHandler<string> ErrorHandler;
    public static event EventHandler<Dictionary<string, UserDataRecord>> DataHandler;

    enum GameDataEventType
    {
        GetUserData,
        UpdateUserData,
        DeleteUserData
    }

    public static string Serialize(object data)
    {
        JsonSerializer Serializer = new();
        Serializer.Converters.Add(new StringEnumConverter());
        Serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        using StringWriter strWriter = new();
        Serializer.Serialize(strWriter, data);
        string serializedValue = strWriter.GetStringBuilder().ToString();
        return serializedValue;
    }

    public static object Deserialize(Type dataType, string dataValue)
    {
        JsonSerializer Serializer = new();
        Serializer.Converters.Add(new StringEnumConverter());
        Serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        StringReader reader = new(dataValue);
        return Serializer.Deserialize(reader, dataType);
    }

    public static void GetUserData(List<GameDataType> gameDataTypes)
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

    public static void UpdateUserData(Dictionary<GameDataType, string> gameDataTypes)
    {
        GameDataEventType eventType = GameDataEventType.UpdateUserData;

        Dictionary<string, string> dataDictionary = new();
        foreach (var gameData in gameDataTypes)
        {
            string serializedKey = gameData.Key.ToString();
            dataDictionary[serializedKey] = gameData.Value;
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

    public static void DeleteUserData(List<GameDataType> gameDataTypes)
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

    public static void DeleteAllUserData()
    {
        List<GameDataType> gameDataTypes = new((GameDataType[])Enum.GetValues(typeof(GameDataType)));
        DeleteUserData(gameDataTypes);
    }

    private static void OnSuccess(GameDataEventType eventType, object result)
    {
        string dataEvent = eventType.ToString();
        SuccessHandler?.Invoke(eventType, dataEvent);

        Debug.Log(dataEvent + " successful");
        if (eventType is GameDataEventType.GetUserData)
        {
            Dictionary<string, UserDataRecord> data;
            data = ((GetUserDataResult)result).Data;
            DataHandler?.Invoke(eventType, data);
        }
    }

    private static void OnError(GameDataEventType eventType, PlayFabError playFabError)
    {
        string errorMsg = playFabError.GenerateErrorReport();
        ErrorHandler?.Invoke(eventType, errorMsg);

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
