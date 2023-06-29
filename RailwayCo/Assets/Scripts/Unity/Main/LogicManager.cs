using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Intermediary between all the GameObjects and Backend GameManeger/GameLogic
public class LogicManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    public List<Cargo> GetTrainCargoList(Guid trainGUID)
    {
        Train trainRef = _gameManager.GameLogic.TrainMaster.GetRef(trainGUID);
        HashSet<Guid> cargoHashset = trainRef.CargoHelper.GetAll();
        return GetCargoListFromGUIDs(cargoHashset);
    }

    public List<Cargo> GetStationCargoList(Guid stationGUID)
    {
        // Gets all the station AND yard cargo, since they are under the same cargoHelper in the station
        HashSet<Guid> cargoHashset = _gameManager.GameLogic.StationMaster.GetRef(stationGUID).CargoHelper.GetAll();

        if (cargoHashset.Count == 0) { 
            // Generate a new set of Cargo if that station is empty
            _gameManager.GameLogic.AddRandomCargoToStation(stationGUID, 10);
            cargoHashset = _gameManager.GameLogic.StationMaster.GetRef(stationGUID).CargoHelper.GetAll();
        }
        return GetCargoListFromGUIDs(cargoHashset);
    }

    private List<Cargo> GetCargoListFromGUIDs(HashSet<Guid> cargoHashset)
    {
        List<Cargo> cargoList = new List<Cargo>();
        foreach (Guid guid in cargoHashset)
        {
            cargoList.Add(_gameManager.GameLogic.CargoMaster.GetRef(guid));
        }
        return cargoList;
    }

    public Station GetIndividualStationInfo(Guid stationGUID)
    {
        return _gameManager.GameLogic.StationMaster.GetRef(stationGUID);
    }

    public void SetStationAsDestination(Guid trainGUID, Guid currentStationGUID, Guid destinationStationGUID)
    {
        _gameManager.GameLogic.OnTrainDeparture(trainGUID, currentStationGUID, destinationStationGUID);
    }

    public void ProcessCargo(Guid trainGUID)
    {
        _gameManager.GameLogic.OnTrainArrival(trainGUID);
        Transform statsPanel = GameObject.Find("MainUI").transform.Find("BottomPanel").Find("UI_StatsPanel");
        int exp = _gameManager.GameLogic.User.ExperiencePoint;
        
        CurrencyManager currMgr = _gameManager.GameLogic.User.CurrencyManager;
        Currency curr;
        currMgr.CurrencyDict.TryGetValue(CurrencyType.Coin, out curr);
        double coinVal = curr.CurrencyValue;

        currMgr.CurrencyDict.TryGetValue(CurrencyType.Note, out curr);
        double noteVal = curr.CurrencyValue;

        currMgr.CurrencyDict.TryGetValue(CurrencyType.NormalCrate, out curr);
        double normalCrateVal = curr.CurrencyValue;

        currMgr.CurrencyDict.TryGetValue(CurrencyType.SpecialCrate, out curr);
        double specialCrateVal = curr.CurrencyValue;

        statsPanel.Find("EXPText").GetComponent<Text>().text = exp.ToString();
        statsPanel.Find("CoinText").GetComponent<Text>().text = coinVal.ToString();
        statsPanel.Find("NoteText").GetComponent<Text>().text = noteVal.ToString();
        statsPanel.Find("NormalCrateText").GetComponent<Text>().text = normalCrateVal.ToString();
        statsPanel.Find("SpecialCrateText").GetComponent<Text>().text = specialCrateVal.ToString();

    }
}
