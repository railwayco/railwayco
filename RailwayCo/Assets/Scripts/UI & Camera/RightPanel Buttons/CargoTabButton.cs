using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoTabButton : MonoBehaviour
{
    public Button cargoButton;

    private void Start()
    {
        cargoButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (cargoButton.name == "StationCargoButton")
        {
            Debug.Log("Station Cargo Button Pressed");
        }
        else if (cargoButton.name == "TrainCargoButton")
        {
            Debug.Log("Train Cargo Button Pressed");
        }
        else
        {
            Debug.LogError("Invalid Button Name");
        }
    }

    
}
