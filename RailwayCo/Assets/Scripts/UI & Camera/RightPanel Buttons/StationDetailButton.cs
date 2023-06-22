using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationDetailButton : MonoBehaviour
{
    [SerializeField] private Button stationButton;
    [SerializeField] private CameraSelection camScript;
    private RightPanelManager rightPanelMgrScript;
    private GameObject stationToFollow;

    void Start()
    {
        GameObject RightPanel = GameObject.FindGameObjectWithTag("MainUI").transform.Find("RightPanel").gameObject;
        rightPanelMgrScript = RightPanel.GetComponent<RightPanelManager>();
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


        GameObject assocTrain = stationToFollow.GetComponent<StationManager>().assocTrain;

        rightPanelMgrScript.loadCargoPanel(assocTrain, stationToFollow, RightPanelManager.CargoTabOptions.NIL);

    }

    public void setStationGameObject(GameObject station)
    {
        stationToFollow = station;
    }
}
