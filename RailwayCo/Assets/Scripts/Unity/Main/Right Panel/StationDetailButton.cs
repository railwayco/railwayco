using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationDetailButton : MonoBehaviour
{
    [SerializeField] private Button _stationButton;
    [SerializeField] private CameraSelection _camScript;
    private RightPanelManager _rightPanelMgrScript;
    private GameObject _stationToFollow;

    void Start()
    {
        GameObject RightPanel = GameObject.FindGameObjectWithTag("MainUI").transform.Find("RightPanel").gameObject;
        _rightPanelMgrScript = RightPanel.GetComponent<RightPanelManager>();
        _stationButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        GameObject worldCamera = _camScript.GetMainCamera();
        if (worldCamera == null)
        {
            Debug.LogError("No World Camera in Scene!");
        }

        worldCamera.GetComponent<WorldCameraMovement>().FollowStation(_stationToFollow);


        GameObject assocTrain = _stationToFollow.GetComponent<StationManager>()._assocTrain;

        _rightPanelMgrScript.LoadCargoPanel(assocTrain, _stationToFollow);

    }

    public void SetStationGameObject(GameObject station)
    {
        _stationToFollow = station;
    }
}
