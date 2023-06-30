using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationDetailButton : MonoBehaviour
{
    [SerializeField] private Button _stationButton;
    private CameraManager _camMgr;
    private RightPanelManager _rightPanelMgrScript;
    private GameObject _stationToFollow;

    void Start()
    {

        GameObject camList = GameObject.Find("CameraList");
        if (camList == null) Debug.LogError("Unable to find Camera List");
        _camMgr = camList.GetComponent<CameraManager>();
        if (!_camMgr) Debug.LogError("There is no Camera Manager attached to the camera list!");

        GameObject RightPanel = GameObject.FindGameObjectWithTag("MainUI").transform.Find("RightPanel").gameObject;
        _rightPanelMgrScript = RightPanel.GetComponent<RightPanelManager>();
        _stationButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        _camMgr.WorldCamFollowStation(_stationToFollow);


        GameObject assocTrain = _stationToFollow.GetComponent<StationManager>()._assocTrain;

        _rightPanelMgrScript.LoadCargoPanel(assocTrain, _stationToFollow);

    }

    // Populate the station button object with the relevant information
    public void SetStationGameObject(GameObject station)
    {
        _stationToFollow = station;
        this.transform.Find("IconRectangle").GetComponent<Image>().sprite = station.GetComponent<SpriteRenderer>().sprite;
        this.transform.Find("StationName").GetComponent<Text>().text = station.name;
    }
}
