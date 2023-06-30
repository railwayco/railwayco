using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoDetailButton : MonoBehaviour
{
    [SerializeField] private GameManager _gameMgr;
    [SerializeField] private Button _cargoDetailButton;
    private LogicManager _logicMgr;
    private Cargo _cargo;
    private Guid _currentTrainGUID;
    private Guid _currentStationGUID;
    // Setup for the Cargo detail button
    public void SetCargoInformation(Cargo cargo, Guid trainguid, Guid stationguid, bool disableCargoDetailButton) 
    {
        _cargo = cargo;
        _currentTrainGUID = trainguid;
        _currentStationGUID = stationguid;
        PopulateCargoInformation(cargo, disableCargoDetailButton);
    }

    private void Awake()
    {
        if (!_cargoDetailButton) Debug.LogError("Cargo Detail button did not reference itself");
        if (!_gameMgr) Debug.LogError("Game Manager not found");

        GameObject lgMgr = GameObject.Find("LogicManager");
        if (!lgMgr) Debug.LogError("Unable to find the Logic Manager");
        _logicMgr = lgMgr.GetComponent<LogicManager>();
        if (!_logicMgr) Debug.LogError("Unable to find the Logic Manager Script");

        _cargoDetailButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        Debug.Log("A Cargo has been clicked");
        if (_currentTrainGUID == Guid.Empty || _currentStationGUID == Guid.Empty) return;

        CargoAssociation cargoAssoc = _cargo.CargoAssoc;
        if (cargoAssoc == CargoAssociation.STATION || cargoAssoc == CargoAssociation.YARD)
        {
            _gameMgr.GameLogic.RemoveCargoFromStation(_currentStationGUID, _cargo.Guid);
            _gameMgr.GameLogic.AddCargoToTrain(_currentTrainGUID, _cargo.Guid);
            // TODO: Check if can add to train before removing from station
            Destroy(this.gameObject);
        } 
        else if (cargoAssoc == CargoAssociation.TRAIN)
        {
            _gameMgr.GameLogic.RemoveCargoFromTrain(_currentTrainGUID, _cargo.Guid);
            _gameMgr.GameLogic.AddCargoToStation(_currentStationGUID, _cargo.Guid);
            // TODO: Check if can add to station before removing from train
            Destroy(this.gameObject);
        }
        else
        {
            Debug.LogError($"There is currently no logic being implemented for CargoAssociation {cargoAssoc}");
        }
    }

    private void PopulateCargoInformation(Cargo cargo, bool disableCargoDetailButton)
    {
        if (disableCargoDetailButton)
        {
            this.GetComponent<Button>().enabled = false;
        }

        Guid destStationGUID = cargo.TravelPlan.DestinationStation;
        string dest = _logicMgr.GetIndividualStationInfo(destStationGUID).Name;
        string cargoType = cargo.Type.ToString();
        string weight = ((int)(cargo.Weight)).ToString();
        string cargoDetail = cargoType + " (" + weight + " t)";

        CurrencyManager currMgr = cargo.CurrencyManager;
        Currency currrency;

        currMgr.CurrencyDict.TryGetValue(CurrencyType.Coin, out currrency);
        string coinAmt = currrency.CurrencyValue.ToString();

        currMgr.CurrencyDict.TryGetValue(CurrencyType.Note, out currrency);
        string noteAmt = currrency.CurrencyValue.ToString();

        currMgr.CurrencyDict.TryGetValue(CurrencyType.NormalCrate, out currrency);
        string nCrateAmt = currrency.CurrencyValue.ToString();

        currMgr.CurrencyDict.TryGetValue(CurrencyType.SpecialCrate, out currrency);
        string sCrateAmt = currrency.CurrencyValue.ToString();

        this.transform.Find("CargoDetails").GetComponent<Text>().text = cargoDetail;
        this.transform.Find("Destination").GetComponent<Text>().text = dest;
        this.transform.Find("CoinAmt").GetComponent<Text>().text = coinAmt;
        this.transform.Find("NoteAmt").GetComponent<Text>().text = noteAmt;
        this.transform.Find("NormalCrateAmt").GetComponent<Text>().text = nCrateAmt;
        this.transform.Find("SpecialCrateAmt").GetComponent<Text>().text = sCrateAmt;
    }

}
