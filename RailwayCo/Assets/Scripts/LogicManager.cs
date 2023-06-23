using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Intermediary between all the GameObjects and Backend GameManeger/GameLogic
public class LogicManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public List<Cargo> getTrainCargoList(Guid trainGUID)
    {
        HashSet<Guid> cargoHashset = gameManager.GameLogic.GetAllCargoGuidsFromTrain(trainGUID);
        return getCargoListFromGUIDs(cargoHashset);
    }

    public List<Cargo> getStationCargoList(Guid stationGUID)
    {
        // Gets all the station AND yard cargo, since they are under the same cargoHelper in the station
        HashSet<Guid> cargoHashset = gameManager.GameLogic.GetAllCargoGuidsFromStation(stationGUID);

        if (cargoHashset.Count == 0) { 
            // Generate a new set of Cargo if that station is empty
            gameManager.GameLogic.AddRandomCargoToStation(stationGUID, 10);
            cargoHashset = gameManager.GameLogic.GetAllCargoGuidsFromStation(stationGUID);
        }
        return getCargoListFromGUIDs(cargoHashset);
    }

    private List<Cargo> getCargoListFromGUIDs(HashSet<Guid> cargoHashset)
    {
        List<Cargo> cargoList = new List<Cargo>();
        foreach (Guid guid in cargoHashset)
        {
            cargoList.Add(gameManager.GameLogic.GetCargoRef(guid));
        }
        return cargoList;
    }

    public Station getIndividualStationInfo(Guid stationGuid)
    {
        return gameManager.GameLogic.GetStationRef(stationGuid);
    }
}
