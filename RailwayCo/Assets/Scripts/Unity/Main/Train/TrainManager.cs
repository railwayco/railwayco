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
    private GameObject _assocPlatform;
    private GameObject _collidedTrain;
    private GameObject _collisionPanel;

    /////////////////////////////////////
    /// INITIALISATION
    /////////////////////////////////////
    private void Awake()
    {
        _collisionPanel = GameObject.Find("UI").transform.Find("CollisionPopupCanvas").Find("CollisionPopupPanel").gameObject;
        if (!_collisionPanel) Debug.LogWarning("Collision Panel Cannot be found");
    }

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
        StartCoroutine(SaveCurrentTrainStatus());
    }

    private IEnumerator SaveCurrentTrainStatus()
    {
        yield return new WaitForSecondsRealtime(5);
        while (true)
        {
            _logicMgr.UpdateTrainBackend(_trainMovementScript, TrainGUID);
            yield return new WaitForSecondsRealtime(30);
        }
    }

    private void OnMouseUpAsButton()
    {
        LoadCargoPanelViaTrain();
        FollowTrain();
    }

    private void UpdateAssocPlatform(GameObject platform)
    {
        GameObject stnCpy;
        if (_assocPlatform && platform) Debug.LogWarning("This scenario should not happen! Will take the passed in parameter");

        if (platform)
        {
            stnCpy = platform;
        }
        else if (_assocPlatform)
        {
            stnCpy = _assocPlatform;
        }
        else
        {
            Debug.LogError("This path should not happen! Either platform or _assocPlatform must be non-null!");
            return;
        }

        // Also help the train to update the PlatformManager of the train status
        PlatformManager stnMgr = stnCpy.GetComponent<PlatformManager>();
        if (platform)
        {
            stnMgr.UpdateAssocTrain(this.gameObject);
        }
        else
        {
            stnMgr.UpdateAssocTrain(null);
        }

        _assocPlatform = platform;
    }

    //////////////////////////////////////////////////////
    // PUBLIC FUNCTIONS
    //////////////////////////////////////////////////////

    public void PlatformEnterProcedure(GameObject platform)
    {
        UpdateAssocPlatform(platform);
        _logicMgr.ProcessCargoOnTrainStop(this.GetComponent<TrainManager>().TrainGUID);

        // Will want to update the TrainOnly panel (and incidentally, StationOnly panel) to TrainStationPanel automatically
        // once the train has docked at the platform (and keep accurate information)
        if (_rightPanelMgrScript.isActiveAndEnabled)
        {
            _rightPanelMgrScript.UpdateChosenCargoTab(RightPanelManager.CargoTabOptions.TRAIN_CARGO);
            _rightPanelMgrScript.LoadCargoPanel(this.gameObject, platform);
        }
    }

    public void PlatformExitProcedure()
    {
        UpdateAssocPlatform(null);
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
        _rightPanelMgrScript.LoadCargoPanel(this.gameObject, _assocPlatform);
    }

    public void FollowTrain()
    {
        _camMgr.WorldCamFollowTrain(this.gameObject);
    }

    public void TrainCollisionCleanupInitiate(GameObject otherTrain)
    {
        Time.timeScale = 0f;
        _collidedTrain = otherTrain;

        
        if (_collisionPanel.activeInHierarchy) return;

        _collisionPanel.SetActive(true);
        _collisionPanel.transform.Find("OKButton").GetComponent<CollisionButton>().SetCaller(this);
    }

    public void TrainCollisionCleanupEnd()
    {
        _logicMgr.OnTrainRemoval(TrainGUID);
        _collisionPanel.SetActive(false);
        Destroy(this.gameObject);
        Destroy(_collidedTrain);
        Time.timeScale = 1f;
    }
}
