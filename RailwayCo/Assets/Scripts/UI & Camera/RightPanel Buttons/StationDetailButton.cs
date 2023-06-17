using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationDetailButton : MonoBehaviour
{
    public Button stationButton;
    public CameraSelection camScript;
    private GameObject stationToFollow;

    void Start()
    {
        stationButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        GameObject worldCamera = camScript.getMainCamera();
        if (worldCamera == null)
        {
            Debug.LogError("No World Camera in Scene!");
        }

        worldCamera.GetComponent<WorldCameraMovement>().followStation(stationToFollow);
    }

    public void setStationGameObject(GameObject station)
    {
        stationToFollow = station;
    }
}
