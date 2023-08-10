using System;
using System.Collections.Generic;
using UnityEngine;

public class CargoManager : MonoBehaviour
{
    private static CargoManager Instance { get; set; }

    [SerializeField] private GameLogic _gameLogic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        if (!Instance._gameLogic) Debug.LogError("Game Logic is not attached to the Cargo Manager");
    }

    //////////////////////////////////////////////////////
    /// CARGO LIST RETRIEVAL
    //////////////////////////////////////////////////////

    public static List<Cargo> GetTrainCargoList(Guid trainGUID)
    {
        if (trainGUID == Guid.Empty)
        {
            Debug.LogError("Invalid trainGUID to get associated cargo");
            return null;
        }

        Train trainRef = Instance._gameLogic.GetTrainObject(trainGUID);
        HashSet<Guid> cargoHashset = trainRef.CargoHelper;
        return GetCargoListFromGUIDs(cargoHashset);
    }

    public static List<Cargo> GetStationCargoList(Guid stationGUID)
    {
        HashSet<Guid> cargoHashset = Instance._gameLogic.GetStationCargoManifest(stationGUID);
        return GetCargoListFromGUIDs(cargoHashset);
    }

    public static List<Cargo> GetYardCargoList(Guid stationGUID)
    {
        HashSet<Guid> cargoHashset = Instance._gameLogic.GetYardCargoManifest(stationGUID);
        return GetCargoListFromGUIDs(cargoHashset);
    }

    private static List<Cargo> GetCargoListFromGUIDs(HashSet<Guid> cargoHashset)
    {
        List<Cargo> cargoList = new();
        foreach (Guid guid in cargoHashset)
        {
            cargoList.Add(Instance._gameLogic.GetCargoObject(guid));
        }
        return cargoList;
    }

    //////////////////////////////////////////////////////
    /// CARGO PROCESSING AND SHIFTING
    //////////////////////////////////////////////////////

    public static bool MoveCargoBetweenTrainAndStation(Cargo cargo, Guid trainGuid, Guid stationGuid)
    {
        CargoAssociation cargoAssoc = cargo.CargoAssoc;
        if (cargoAssoc == CargoAssociation.Station || cargoAssoc == CargoAssociation.Yard)
        {
            if (!Instance._gameLogic.AddCargoToTrain(trainGuid, cargo.Guid))
                return false;
            Instance._gameLogic.RemoveCargoFromStation(stationGuid, cargo.Guid);
        }
        else if (cargoAssoc == CargoAssociation.Train)
        {
            if (!Instance._gameLogic.AddCargoToStation(stationGuid, cargo.Guid))
                return false;
            Instance._gameLogic.RemoveCargoFromTrain(trainGuid, cargo.Guid);
        }
        else
        {
            Debug.LogError($"There is currently no logic being implemented for CargoAssociation {cargoAssoc}");
            return false;
        }
        return true;
    }

    public static void ProcessTrainCargo(Guid trainGUID, Guid stationGuid)
    {
        if (Instance._gameLogic.GetTrainDepartureStation(trainGUID) == default)
            Instance._gameLogic.OnTrainRestoration(trainGUID, stationGuid);
        else
        {
            Instance._gameLogic.OnTrainArrival(trainGUID);
            UserManager.UpdateUserStatsPanel();
        }
    }
}
