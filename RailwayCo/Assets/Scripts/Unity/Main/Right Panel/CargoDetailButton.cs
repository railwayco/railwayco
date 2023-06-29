using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoDetailButton : MonoBehaviour
{
    [SerializeField] private GameManager gameMgr;
    [SerializeField] private Button cargoInfo;
    private Cargo cargo;
    private Guid trainGUID;
    private Guid stationGUID;
    public void setCargoCellInformation(Cargo c, Guid trainguid, Guid stationguid) 
    {
        cargo = c;
        trainGUID = trainguid;
        stationGUID = stationguid;
    }

    void Start()
    { 
        cargoInfo.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        Debug.Log("A Cargo has been clicked");
        if (trainGUID == Guid.Empty || stationGUID == Guid.Empty) return;

        CargoAssociation cargoAssoc = cargo.CargoAssoc;
        if (cargoAssoc == CargoAssociation.STATION || cargoAssoc == CargoAssociation.YARD)
        {
            gameMgr.GameLogic.RemoveCargoFromStation(stationGUID, cargo.Guid);
            gameMgr.GameLogic.AddCargoToTrain(trainGUID, cargo.Guid);
            // TODO: Check if can add to train before removing from station
            Destroy(this.gameObject);
        } 
        else if (cargoAssoc == CargoAssociation.TRAIN)
        {
            gameMgr.GameLogic.RemoveCargoFromTrain(trainGUID, cargo.Guid);
            gameMgr.GameLogic.AddCargoToStation(stationGUID, cargo.Guid);
            // TODO: Check if can add to station before removing from train
            Destroy(this.gameObject);
        }
        else
        {
            Debug.LogError($"There is currently no logic being implemented for CargoAssociation {cargoAssoc}");
        }
    }

}
