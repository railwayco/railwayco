using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the behaviour of the 2 cargo Tab Buttons under the CargoTrainStationPanel
/// </summary>
public class CargoTabButton : MonoBehaviour
{
    public Button cargoButton;
    private RightPanelManager rightPanelMgrScript;

    // Depending on the cargoButton that this script is associated with, either one will be set to Guid.Empty by the RightPanel manager when
    private GameObject station;
    private GameObject train;

    private void Start()
    {
        GameObject RightPanel = GameObject.FindGameObjectWithTag("MainUI").transform.Find("RightPanel").gameObject;
        rightPanelMgrScript = RightPanel.GetComponent<RightPanelManager>();
        cargoButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (cargoButton.name == "StationCargoButton")
        {
            Debug.Log("Station Cargo Button Pressed");
            rightPanelMgrScript.setChosenCargoTab(RightPanelManager.CargoTabOptions.STATION_CARGO);
            rightPanelMgrScript.loadCargoPanel(train, station);
        }
        else if (cargoButton.name == "TrainCargoButton")
        {
            Debug.Log("Train Cargo Button Pressed");
            rightPanelMgrScript.setChosenCargoTab(RightPanelManager.CargoTabOptions.TRAIN_CARGO);
            rightPanelMgrScript.loadCargoPanel(train, station);
        }
        else
        {
            Debug.LogError("Invalid Button Name");
        }
    }

    public void setTrainAndStationGameObj(GameObject trainObject, GameObject stationObject)
    {
        train = trainObject;
        station = stationObject;
    }

    
}
