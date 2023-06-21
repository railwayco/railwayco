using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Intermediary between all the GameObjects and Backend GameManeger/GameLogic
public class LogicManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private Guid stationGUID;
    private void Start()
    {
        // Stop-Gap solution until the station can somehow manage their GUID and stuff
        HashSet<Guid> stationGUIDs = gameManager.GameLogic.GetAllStationGuids();
        foreach (Guid guid in stationGUIDs)
        {
            stationGUID = guid;
        }
        gameManager.GameLogic.AddRandomCargoToStation(stationGUID, 5);
    }




    public Cargo[] getStationCargoList()
    {
        // Get the top i from the MASTER cargo list for all stations
        // for now, given the way its been implemented, source station will be the same (wrong) one
        // To be fixed in subsequent iterations. (The logic to determine the right source station)
        HashSet<Guid> cargoHashset = gameManager.GameLogic.GetAllCargoGuidsFromStation(stationGUID);
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
