using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Intermediary between all the GameObjects and Backend GameManeger/GameLogic
public class LogicManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public List<Cargo> getTrainCargoList(Guid trainGUID)
    {
        Train trainRef = gameManager.GameLogic.TrainMaster.GetRef(trainGUID);
        HashSet<Guid> cargoHashset = trainRef.CargoHelper.GetAll();
        return getCargoListFromGUIDs(cargoHashset);
    }

    public List<Cargo> getStationCargoList(Guid stationGUID)
    {
        // Gets all the station AND yard cargo, since they are under the same cargoHelper in the station
        HashSet<Guid> cargoHashset = gameManager.GameLogic.StationMaster.GetRef(stationGUID).CargoHelper.GetAll();

        if (cargoHashset.Count == 0) { 
            // Generate a new set of Cargo if that station is empty
            gameManager.GameLogic.AddRandomCargoToStation(stationGUID, 10);
            cargoHashset = gameManager.GameLogic.StationMaster.GetRef(stationGUID).CargoHelper.GetAll();
        }
        return getCargoListFromGUIDs(cargoHashset);
    }

    private List<Cargo> getCargoListFromGUIDs(HashSet<Guid> cargoHashset)
    {
        List<Cargo> cargoList = new List<Cargo>();
        foreach (Guid guid in cargoHashset)
        {
            cargoList.Add(gameManager.GameLogic.CargoMaster.GetRef(guid));
        }
        return cargoList;
    }

    public Station getIndividualStationInfo(Guid stationGuid)
    {
        return gameManager.GameLogic.StationMaster.GetRef(stationGuid);
    }

    public void setStationAsDestination(Guid trainGUid, Guid currentStationGuid, Guid destinationStationGuid)
    {
        gameManager.GameLogic.OnTrainDeparture(trainGUid, currentStationGuid, destinationStationGuid);
    }

    public void processCargo(Guid trainGUID)
    {
        gameManager.GameLogic.OnTrainArrival(trainGUID);
        Transform StatsPanel = GameObject.Find("MainUI").transform.Find("BottomPanel").Find("UI_StatsPanel");
        int exp = gameManager.GameLogic.User.ExperiencePoint;
        
        CurrencyManager currMgr = gameManager.GameLogic.User.CurrencyManager;
        Currency curr;
        currMgr.CurrencyDict.TryGetValue(CurrencyType.Coin, out curr);
        double coinVal = curr.CurrencyValue;

        currMgr.CurrencyDict.TryGetValue(CurrencyType.Note, out curr);
        double noteVal = curr.CurrencyValue;

        currMgr.CurrencyDict.TryGetValue(CurrencyType.NormalCrate, out curr);
        double normalCrateVal = curr.CurrencyValue;

        currMgr.CurrencyDict.TryGetValue(CurrencyType.SpecialCrate, out curr);
        double specialCrateVal = curr.CurrencyValue;

        StatsPanel.Find("EXPText").GetComponent<Text>().text = exp.ToString();
        StatsPanel.Find("CoinText").GetComponent<Text>().text = coinVal.ToString();
        StatsPanel.Find("NoteText").GetComponent<Text>().text = noteVal.ToString();
        StatsPanel.Find("NormalCrateText").GetComponent<Text>().text = normalCrateVal.ToString();
        StatsPanel.Find("SpecialCrateText").GetComponent<Text>().text = specialCrateVal.ToString();

    }
}
