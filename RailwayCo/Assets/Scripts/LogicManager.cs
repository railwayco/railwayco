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

    // Only a temp measure
    public void setStation1AsDestionation(Guid trainGUid, Guid currentStationGuid)
    {
        Debug.LogWarning("This is a hardcoded function, and has to be replaced with something dynamic.");
        GameObject go = GameObject.Find("Station1");
        Guid destStnGUID = go.GetComponent<StationManager>().stationGUID;
        gameManager.GameLogic.SetTrainTravelPlan(trainGUid, currentStationGuid, destStnGUID);
    }

    public void processCargo(Guid trainGUID)
    {
        gameManager.GameLogic.OnTrainArrival(trainGUID);
        Transform StatsPanel = GameObject.Find("MainUI").transform.Find("BottomPanel").Find("UI_StatsPanel");
        int exp = gameManager.GameLogic.GetUserExperiencePoints();
        
        CurrencyManager currMgr = gameManager.GameLogic.GetUserCurrencyManager();
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
