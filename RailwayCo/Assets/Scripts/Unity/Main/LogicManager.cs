using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Intermediary between all the GameObjects and Backend GameLogic
public class LogicManager : MonoBehaviour
{
    [SerializeField] private GameLogic _gameLogic;
    private Coroutine _sendDataToPlayfabCoroutine;

    private void Awake()
    {
        if (!_gameLogic) Debug.LogError("Game Logic is not attached to the logic manager!");
        _sendDataToPlayfabCoroutine = StartCoroutine(SendDataToPlayfabRoutine(60f));
    }

    //////////////////////////////////////////////////////
    /// PLAYFAB RELATED
    //////////////////////////////////////////////////////

    private IEnumerator SendDataToPlayfabRoutine(float secondsTimeout)
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsTimeout);
            _gameLogic.SendDataToPlayfab();
        }

        // TODO: Graceful termination when signalled by
        // OnApplicationPause or OnApplicationQuit
        // that will be implemented using StopCoroutine
    }

    //////////////////////////////////////////////////////
    /// CARGO LIST RETRIEVAL
    //////////////////////////////////////////////////////

    public List<Cargo> GetTrainCargoList(Guid trainGUID)
    {
        if (trainGUID == Guid.Empty)
        {
            Debug.LogError("Invalid trainGUID to get associated cargo");
            return null;
        }

        Train trainRef = _gameLogic.GetTrainObject(trainGUID);
        HashSet<Guid> cargoHashset = trainRef.CargoHelper;
        return GetCargoListFromGUIDs(cargoHashset);
    }

    public List<Cargo> GetStationCargoList(Guid stationGUID)
    {
        HashSet<Guid> cargoHashset = _gameLogic.GetStationCargoManifest(stationGUID);
        return GetCargoListFromGUIDs(cargoHashset);
    }

    public List<Cargo> GetYardCargoList(Guid stationGUID)
    {
        HashSet<Guid> cargoHashset = _gameLogic.GetYardCargoManifest(stationGUID);
        return GetCargoListFromGUIDs(cargoHashset);
    }

    private List<Cargo> GetCargoListFromGUIDs(HashSet<Guid> cargoHashset)
    {
        List<Cargo> cargoList = new();
        foreach (Guid guid in cargoHashset)
        {
            cargoList.Add(_gameLogic.GetCargoObject(guid));
        }
        return cargoList;
    }

    //////////////////////////////////////////////////////
    /// CARGO PROCESSING AND SHIFTING
    //////////////////////////////////////////////////////

    public bool MoveCargoBetweenTrainAndStation(Cargo cargo, Guid trainGuid, Guid stationGuid)
    {
        CargoAssociation cargoAssoc = cargo.CargoAssoc;
        if (cargoAssoc == CargoAssociation.Station || cargoAssoc == CargoAssociation.Yard)
        {
            if (!_gameLogic.AddCargoToTrain(trainGuid, cargo.Guid))
                return false;
            _gameLogic.RemoveCargoFromStation(stationGuid, cargo.Guid);
        }
        else if (cargoAssoc == CargoAssociation.Train)
        {
            if (!_gameLogic.AddCargoToStation(stationGuid, cargo.Guid))
                return false;
            _gameLogic.RemoveCargoFromTrain(trainGuid, cargo.Guid);
        }
        else
        {
            Debug.LogError($"There is currently no logic being implemented for CargoAssociation {cargoAssoc}");
            return false;
        }
        return true;
    }


    //////////////////////////////////////////////////////
    /// UNLOCKING RELATED
    //////////////////////////////////////////////////////

    public bool AbleToPurchase(CurrencyManager cost)
    {
        if (_gameLogic.RemoveUserCurrencyManager(cost))
        {
            UserManager.UpdateUserStatsPanel();
            return true;
        }
        return false;
    }
}
