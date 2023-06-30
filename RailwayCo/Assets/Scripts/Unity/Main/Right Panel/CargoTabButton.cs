using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the behaviour of the 3 cargo Tab Buttons under the CargoTrainStationPanel
/// </summary>
public class CargoTabButton : MonoBehaviour
{
    [SerializeField] private Button _cargoButton;
    private RightPanelManager _rightPanelMgrScript;

    // Depending on the cargoButton that this script is associated with, either one will be set to Guid.Empty by the RightPanel manager when
    private GameObject _station;
    private GameObject _train;

    public void SetCargoTabButtonInformation(GameObject trainObject, GameObject stationObject)
    {
        _train = trainObject;
        _station = stationObject;
    }

    private void Start()
    {
        GameObject RightPanel = GameObject.FindGameObjectWithTag("MainUI").transform.Find("RightPanel").gameObject;
        _rightPanelMgrScript = RightPanel.GetComponent<RightPanelManager>();
        _cargoButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (_cargoButton.name == "StationCargoButton")
        {
            Debug.Log("Station Cargo Button Pressed");
            _rightPanelMgrScript.UpdateChosenCargoTab(RightPanelManager.CargoTabOptions.STATION_CARGO);
            
        }
        else if (_cargoButton.name == "TrainCargoButton")
        {
            Debug.Log("Train Cargo Button Pressed");
            _rightPanelMgrScript.UpdateChosenCargoTab(RightPanelManager.CargoTabOptions.TRAIN_CARGO);
            
        }
        else if (_cargoButton.name == "YardCargoButton")
        {
            Debug.Log("Yard Cargo Button pressed");
            _rightPanelMgrScript.UpdateChosenCargoTab(RightPanelManager.CargoTabOptions.YARD_CARGO);
        }
        else
        {
            Debug.LogError("Invalid Button Name");
            return;
        }
        _rightPanelMgrScript.LoadCargoPanel(_train, _station);
    }

    

    
}
