using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    private LogicManager _logicMgr;
    private CameraManager _camMgr;
    private TrainMovement _trainMovementScript;
    private RightPanelManager _rightPanelMgrScript;
    public Guid TrainGUID { get; private set; } // Exposed to uniquely identify the train
    private GameObject _assocStation;

    public void StationEnterProcedure(GameObject station)
    {
        UpdateAssocStation(station);
        _logicMgr.ProcessCargo(this.GetComponent<TrainManager>().TrainGUID);

    }

    public void StationExitProcedure(GameObject station)
    {
        UpdateAssocStation(station);
    }


    private void UpdateAssocStation(GameObject station)
    {
        GameObject stnCpy = null;
        if (_assocStation && station) Debug.LogWarning("This scenario should not happen! Will take the parameter");

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
        
        StationManager stnMgr = stnCpy.GetComponent<StationManager>();
        if (station)
        {
            stnMgr.UpdateAssocTrain(this.gameObject);
            _gameManager.GameLogic.SetTrainTravelPlan(TrainGUID, stnMgr.StationGUID, stnMgr.StationGUID);
        }
        else
        {
            stnMgr.UpdateAssocTrain(null);
            // TODO: setup train travel plan here
        }

        _assocStation = station;
    }

    /////////////////////////////////////
    /// INITIALISATION
    /////////////////////////////////////
    void Start()
    {

        GameObject camList = GameObject.Find("CameraList");
        if (camList == null) Debug.LogError("Unable to find Camera List");
        _camMgr = camList.GetComponent<CameraManager>();
        if (!_camMgr) Debug.LogError("There is no Camera Manager attached to the camera list!");

        GameObject rightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        _rightPanelMgrScript = rightPanel.GetComponent<RightPanelManager>();
        _trainMovementScript = this.gameObject.GetComponent<TrainMovement>();

        _logicMgr = GameObject.FindGameObjectsWithTag("Logic")[0].GetComponent<LogicManager>();

        // Stop-Gap Solution until Save/Load features are properly implemented
        Guid trainGuid;

        Vector3 position = gameObject.transform.position;
        Train train = _gameManager.GameLogic.GetTrainRefByPosition(position);

        TrainDirection movementDirn = _trainMovementScript.MovementDirn;
        Vector3 trainPosition = _trainMovementScript.transform.position;
        Quaternion trainRotation = _trainMovementScript.transform.rotation;
        float maxSpeed = _trainMovementScript.MaxSpeed;

        if (train is null)
        {
            trainGuid = _gameManager.GameLogic.InitTrain(this.name, maxSpeed, trainPosition, trainRotation, movementDirn);
        }
        else
        {
            trainGuid = train.Guid;
        }

        TrainGUID = trainGuid;
    }

    void Update()
    {
        float trainCurrentSpeed = _trainMovementScript.CurrentSpeed;
        TrainDirection movementDirn = _trainMovementScript.MovementDirn;
        Vector3 trainPosition = _trainMovementScript.transform.position;
        Quaternion trainRotation = _trainMovementScript.transform.rotation;
        Train trainObject = _gameManager.GameLogic.TrainMaster.GetObject(TrainGUID);
        trainObject.Attribute.SetUnityStats(trainCurrentSpeed, trainPosition, trainRotation, movementDirn);
    }

    private void OnMouseUpAsButton()
    {
        LoadCargoPanelViaTrain();
        FollowTrain();
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
