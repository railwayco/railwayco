using System;
using System.Collections;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    private LogicManager _logicMgr;
    private CameraManager _camMgr;
    private TrainMovement _trainMovementScript;
    private RightPanelManager _rightPanelMgrScript;
    public Guid TrainGUID { get; private set; } // Exposed to uniquely identify the train
    private GameObject _assocStation;

    /////////////////////////////////////
    /// INITIALISATION
    /////////////////////////////////////
    private void Start()
    {

        GameObject camList = GameObject.Find("CameraList");
        if (camList == null) Debug.LogError("Unable to find Camera List");
        _camMgr = camList.GetComponent<CameraManager>();
        if (!_camMgr) Debug.LogError("There is no Camera Manager attached to the camera list!");

        GameObject rightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        _rightPanelMgrScript = rightPanel.GetComponent<RightPanelManager>();
        _trainMovementScript = this.gameObject.GetComponent<TrainMovement>();

        _logicMgr = GameObject.FindGameObjectsWithTag("Logic")[0].GetComponent<LogicManager>();

        // Stop-Gap Solution until Save/Load features are properly implemented so that we can stop passing in the script reference.
        TrainGUID = _logicMgr.SetupGetTrainGUID(_trainMovementScript, this.gameObject);
    }

    private void OnMouseUpAsButton()
    {
        LoadCargoPanelViaTrain();
        FollowTrain();
    }

    private void UpdateAssocStation(GameObject station)
    {
        GameObject stnCpy;
        if (_assocStation && station) Debug.LogWarning("This scenario should not happen! Will take the passed in parameter");

        if (station)
        {
            stnCpy = station;
        }
        else if (_assocStation)
        {
            stnCpy = _assocStation;
        }
        else
        {
            Debug.LogError("This path should not happen! Either station or _assocStation must be non-null!");
            return;
        }

        // Also help the train to update the Station Manager of the train status
        StationManager stnMgr = stnCpy.GetComponent<StationManager>();
        if (station)
        {
            stnMgr.UpdateAssocTrain(this.gameObject);
        }
        else
        {
            stnMgr.UpdateAssocTrain(null);
            // TODO: setup train travel plan here
        }

        _assocStation = station;
    }

    //////////////////////////////////////////////////////
    // PUBLIC FUNCTIONS
    //////////////////////////////////////////////////////

    public void StationEnterProcedure(GameObject station)
    {
        UpdateAssocStation(station);
        _logicMgr.ProcessCargoOnTrainStop(this.GetComponent<TrainManager>().TrainGUID);
    }

    public void StationExitProcedure(GameObject station)
    {
        UpdateAssocStation(station);
    }

    public void SaveCurrentTrainStatus()
    {
        _logicMgr.UpdateTrainBackend(_trainMovementScript,TrainGUID);   
    }

    public IEnumerator ReplenishTrainFuelAndDurability()
    {
        Guid trainGUID = GetComponent<TrainManager>().TrainGUID;
        for (;;)
        {
            yield return new WaitForSeconds(30);
            _logicMgr.ReplenishTrainFuelAndDurability(trainGUID);
        }
    }

    public void LoadCargoPanelViaTrain()
    {
        _rightPanelMgrScript.LoadCargoPanel(this.gameObject, _assocStation);
    }

    public void FollowTrain()
    {
        _camMgr.WorldCamFollowTrain(this.gameObject);
    }
}
