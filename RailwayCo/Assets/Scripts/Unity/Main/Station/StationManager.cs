using System;
using UnityEngine;


public class StationManager : MonoBehaviour
{
    private LogicManager _logicMgr;
    private CameraManager _camMgr;
    private RightPanelManager _rightPanelMgr;

    public Guid StationGUID { get; private set; } // Exposed to uniquely identify the station
    private bool _isNewStation;
    private GameObject _assocTrain; // Need the Train side to tell the station that it has arrived

    // Called by the train when it stops at the station and right when it moves
    // This is to allow for the correct cargo panel to be loaded.
    public void UpdateAssocTrain(GameObject train)
    {
        _assocTrain = train;
    }

    /////////////////////////////////////
    /// INITIALISATION
    /////////////////////////////////////
    private void Awake()
    {
        GameObject camList = GameObject.Find("CameraList");
        if (camList == null) Debug.LogError("Unable to find Camera List");
        _camMgr = camList.GetComponent<CameraManager>();
        if (!_camMgr) Debug.LogError("There is no Camera Manager attached to the camera list!");

        GameObject RightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        _rightPanelMgr = RightPanel.GetComponent<RightPanelManager>();

        _logicMgr = GameObject.Find("LogicManager").GetComponent<LogicManager>();
        if (!_logicMgr) Debug.LogError($"LogicManager is not present in the scene");

        StationGUID = _logicMgr.SetupGetStationGUID(out _isNewStation, this.gameObject);    
    }

    private void Start()
    {
        if (_isNewStation) _logicMgr.StationGenerateTracks(this.name);
    }

    private void OnMouseUpAsButton()
    {
        LoadCargoPanelViaStation();
    }

    //////////////////////////////////////////////////////
    // PUBLIC FUNCTIONS
    //////////////////////////////////////////////////////

   

    public void followStation()
    {
        _camMgr.WorldCamFollowStation(this.gameObject);
    }

    public void LoadCargoPanelViaStation()
    {
        _rightPanelMgr.LoadCargoPanel(_assocTrain, this.gameObject);
    }
}
