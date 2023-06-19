using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoTabButton : MonoBehaviour
{
    private GameObject trainCargoPanel;
    private GameObject stationCargoPanel;

    private void Start()
    {
        trainCargoPanel = GameObject.Find("TrainCargoPanel");
        stationCargoPanel = GameObject.Find("StationCargoPanel");
    }

    public void OnTrainCargoButtonClicked()
    {
        trainCargoPanel.SetActive(true);
        stationCargoPanel.SetActive(false);
    }

    public void OnStationCargoButtonClicked()
    {
        trainCargoPanel.SetActive(false);
        stationCargoPanel.SetActive(true);
    }
}
