using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoDetailButton : MonoBehaviour
{
    [SerializeField] private GameManager _gameMgr;
    [SerializeField] private Button _cargoInfo;
    private Cargo _cargo;
    private Guid _trainGUID;
    private Guid _stationGUID;
    public void SetCargoCellInformation(Cargo cargo, Guid trainguid, Guid stationguid) 
    {
        _cargo = cargo;
        _trainGUID = trainguid;
        _stationGUID = stationguid;
    }

    void Start()
    { 
        _cargoInfo.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        Debug.Log("A Cargo has been clicked");
        if (_trainGUID == Guid.Empty || _stationGUID == Guid.Empty) return;

        CargoAssociation cargoAssoc = _cargo.CargoAssoc;
        if (cargoAssoc == CargoAssociation.STATION || cargoAssoc == CargoAssociation.YARD)
        {
            _gameMgr.GameLogic.RemoveCargoFromStation(_stationGUID, _cargo.Guid);
            _gameMgr.GameLogic.AddCargoToTrain(_trainGUID, _cargo.Guid);
            // TODO: Check if can add to train before removing from station
            Destroy(this.gameObject);
        } 
        else if (cargoAssoc == CargoAssociation.TRAIN)
        {
            _gameMgr.GameLogic.RemoveCargoFromTrain(_trainGUID, _cargo.Guid);
            _gameMgr.GameLogic.AddCargoToStation(_stationGUID, _cargo.Guid);
            // TODO: Check if can add to station before removing from train
            Destroy(this.gameObject);
        }
        else
        {
            Debug.LogError($"There is currently no logic being implemented for CargoAssociation {cargoAssoc}");
        }
    }

}
