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

    private JsonSerializer Serializer { get; set; }

    public GameDataManager()
    {
        Serializer = new();
        Serializer.Converters.Add(new StringEnumConverter());
        Serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    }

    enum GameDataEventType
    {
        GetUserData,
        UpdateUserData,
        DeleteUserData
    }

    public string Serialize(object data)
    {
        using StringWriter strWriter = new();
        Serializer.Serialize(strWriter, data);
        string serializedValue = strWriter.GetStringBuilder().ToString();
        return serializedValue;
    }

    public object Deserialize(GameDataType dataType, string dataValue)
    {
        StringReader reader = new(dataValue);

        switch (dataType)
        {
            case GameDataType.User:
                {
                    return Serializer.Deserialize(reader, typeof(User));
                }
            case GameDataType.CargoMaster:
                {
                    return Serializer.Deserialize(reader, typeof(WorkerDictHelper<Cargo>));
                }
            case GameDataType.CargoCatalog:
                {
                    return Serializer.Deserialize(reader, typeof(WorkerDictHelper<CargoModel>));
                }
            case GameDataType.TrainMaster:
                {
                    return Serializer.Deserialize(reader, typeof(WorkerDictHelper<Train>));
                }
            case GameDataType.TrainCatalog:
                {
                    return Serializer.Deserialize(reader, typeof(WorkerDictHelper<TrainModel>));
                }
            case GameDataType.StationMaster:
                {
                    return Serializer.Deserialize(reader, typeof(WorkerDictHelper<Station>));
                }
            case GameDataType.StationReacher:
                {
                    return Serializer.Deserialize(reader, typeof(StationReacher));
                }
            default:
                {
                    Debug.LogError("Unknown GameDataType. Unable to deserialize.");
                    return null;
                }
        }
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

    public void UpdateUserData(Dictionary<GameDataType, string> gameDataTypes)
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
