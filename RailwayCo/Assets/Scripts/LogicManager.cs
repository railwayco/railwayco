using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Intermediary between all the GameObjects and Backend GameManeger/GameLogic
public class LogicManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    
    //public Cargo[] getTrainCargoList()
    //{

    //}

    public Cargo[] getStationCargoList(Guid stationGUID)
    {
        HashSet<Guid> cargoHashset = gameManager.GameLogic.GetAllCargoGuidsFromStation(stationGUID);

        if (cargoHashset.Count == 0) { 
            // Generate a new set of Cargo if that station is empty
            gameManager.GameLogic.AddRandomCargoToStation(stationGUID, 30);
            cargoHashset = gameManager.GameLogic.GetAllCargoGuidsFromStation(stationGUID);
        }

    Cargo[] cargoList = new Cargo[cargoHashset.Count];
        int readCount = 0;
        foreach (Guid guid in cargoHashset)
        {
            if (readCount >= cargoHashset.Count) break;
            cargoList[readCount] = gameManager.GameLogic.GetCargoRef(guid);
            readCount++;
        }
        return cargoList;
    }

    public Station getIndividualStationInfo(Guid stationGuid)
    {
        return gameManager.GameLogic.GetStationRef(stationGuid);
    }
}
