using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CameraSelection camScript;
    private TrainMovement trainMovementScript;
    private RightPanelManager rightPanelMgrScript;
    public Guid trainGUID { get; private set; }
    public Guid currentStnGUID { get; private set; }

    void Start()
    {
        GameObject RightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        rightPanelMgrScript = RightPanel.GetComponent<RightPanelManager>();
        trainMovementScript = this.gameObject.GetComponent<TrainMovement>();

        // Stop-Gap Solution until Save/Load features are properly implemented
        Guid trainGuid;

        Vector3 position = gameObject.transform.position;
        Train train = gameManager.GameLogic.GetTrainRefByPosition(position);

        TrainDirection movementDirn = trainMovementScript.MovementDirn;
        Vector3 trainPosition = trainMovementScript.transform.position;
        Quaternion trainRotation = trainMovementScript.transform.rotation;
        float maxSpeed = trainMovementScript.MaxSpeed;

        if (train is null)
        {
            trainGuid = gameManager.GameLogic.InitTrain(this.name, maxSpeed, trainPosition, trainRotation, movementDirn);
        }
        else
        {
            trainGuid = train.Guid;
        }
        setTrainGUID(trainGuid);
    }

    void Update()
    {
        float trainCurrentSpeed = trainMovementScript.CurrentSpeed;
        TrainDirection movementDirn = trainMovementScript.MovementDirn;
        Vector3 trainPosition = trainMovementScript.transform.position;
        Quaternion trainRotation = trainMovementScript.transform.rotation;
        gameManager.GameLogic.SetTrainUnityStats(trainGUID, trainCurrentSpeed, trainPosition, trainRotation, movementDirn);
    }

    public void setTrainGUID(Guid TrnGUID)
    {
        trainGUID = TrnGUID;
    }

    public void setCurrentStationGUID(Guid stnGUID)
    {
        currentStnGUID = stnGUID;
    }

    public void setTrainTravelPlan(Guid sourceStationGuid, Guid destinationStationGuid)
    {
        gameManager.GameLogic.SetTrainTravelPlan(trainGUID, sourceStationGuid, destinationStationGuid);
    }

    private void OnMouseUpAsButton()
    {
        GameObject station = trainMovementScript.CurrentStation;
        rightPanelMgrScript.loadCargoPanel(this.gameObject, station);
        followTrain();
    }

    public void followTrain()
    {
        GameObject worldCamera = camScript.getMainCamera();
        if (worldCamera == null)
        {
            Debug.LogError("No World Camera in Scene!");
        }

        worldCamera.GetComponent<WorldCameraMovement>().followtrain(this.gameObject);
    }
}
