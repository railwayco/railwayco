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

    void Start()
    {
        GameObject RightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        rightPanelMgrScript = RightPanel.GetComponent<RightPanelManager>();
        trainMovementScript = this.gameObject.GetComponent<TrainMovement>();
        
        // Stop-Gap Solution until Save/Load features are properly implemented
        Guid trainGuid = gameManager.GameLogic.saveTrainInfo(this.name);
        setTrainGUID(trainGuid);
    }

    // This function should only be set by LogicManager and nowhere else
    public void setTrainGUID(Guid TrnGUID)
    {
        trainGUID = TrnGUID;
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
