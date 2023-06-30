using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    private CameraManager _camMgr;
    private TrainMovement _trainMovementScript;
    private RightPanelManager _rightPanelMgrScript;
    public Guid TrainGUID { get; private set; }
    public Guid CurrentStnGUID { get; private set; }

    void Start()
    {

        GameObject camList = GameObject.Find("CameraList");
        if (camList == null) Debug.LogError("Unable to find Camera List");
        _camMgr = camList.GetComponent<CameraManager>();
        if (!_camMgr) Debug.LogError("There is no Camera Manager attached to the camera list!");

        GameObject rightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        _rightPanelMgrScript = rightPanel.GetComponent<RightPanelManager>();
        _trainMovementScript = this.gameObject.GetComponent<TrainMovement>();

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
        SetTrainGUID(trainGuid);
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

    public void SetTrainGUID(Guid trainGUID)
    {
       TrainGUID = trainGUID;
    }

    public void SetCurrentStationGUID(Guid stnGUID)
    {
        CurrentStnGUID = stnGUID;
    }

    public void SetTrainTravelPlan(Guid sourceStationGUID, Guid destinationStationGUID)
    {
        _gameManager.GameLogic.SetTrainTravelPlan(TrainGUID, sourceStationGUID, destinationStationGUID);
    }

    private void OnMouseUpAsButton()
    {
        GameObject station = _trainMovementScript.CurrentStation;
        _rightPanelMgrScript.LoadCargoPanel(this.gameObject, station);
        FollowTrain();
    }

    public void FollowTrain()
    {
        _camMgr.WorldCamFollowTrain(this.gameObject);
    }
}
