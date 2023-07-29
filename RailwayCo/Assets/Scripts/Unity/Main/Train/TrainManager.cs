using System;
using System.Collections;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    private LogicManager _logicMgr;
    private CameraManager _camMgr;
    private TrainMovement _trainMovementScript;
    private RightPanelManager _rightPanelMgr;
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

        GameObject rightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        _rightPanelMgr = rightPanel.GetComponent<RightPanelManager>();

        _logicMgr = GameObject.FindGameObjectsWithTag("Logic")[0].GetComponent<LogicManager>();
        TrainGUID = _logicMgr.GetTrainClassObject(this.gameObject.transform.position).Guid;
    }

    private void Start()
    {
        GameObject camList = GameObject.Find("CameraList");
        if (camList == null) Debug.LogError("Unable to find Camera List");
        _camMgr = camList.GetComponent<CameraManager>();
        if (!_camMgr) Debug.LogError("There is no Camera Manager attached to the camera list!");

        _trainMovementScript = this.gameObject.GetComponent<TrainMovement>();
        StartCoroutine(SaveCurrentTrainStatus());
    }

    public IEnumerator SaveCurrentTrainStatus()
    {
        while (true)
        {
            _logicMgr.UpdateTrainBackend(_trainMovementScript.TrainAttribute, TrainGUID);
            yield return new WaitForSecondsRealtime(5);
        }
    }

    public TrainAttribute GetTrainAttribute()
    {
        TrainAttribute trainAttribute = _logicMgr.GetTrainAttribute(TrainGUID);
        return trainAttribute;
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
        Guid stationGuid = platform.GetComponent<PlatformManager>().StationGUID;
        _logicMgr.ProcessCargoOnTrainStop(this.GetComponent<TrainManager>().TrainGUID, stationGuid);

        // Will want to update the TrainOnly panel (and incidentally, StationOnly panel) to TrainStationPanel automatically
        // once the train has docked at the platform (and keep accurate information)
        if (_rightPanelMgr.isActiveAndEnabled)
        {
            if (!_rightPanelMgr.IsActivePanelSamePanelType(RightPanelType.Cargo))
                return;

            if (!_rightPanelMgr.IsActiveCargoPanelSameTrainOrPlatform(this.gameObject, platform))
                return;

            _rightPanelMgr.LoadCargoPanel(this.gameObject, platform, CargoTabOptions.TrainCargo);
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
        _rightPanelMgr.LoadCargoPanel(this.gameObject, _assocPlatform, CargoTabOptions.Nil);
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
        _logicMgr.OnTrainCollision(TrainGUID);
        _logicMgr.OnTrainCollision(_collidedTrain.GetComponent<TrainManager>().TrainGUID);

        _collisionPanel.SetActive(false);
        Destroy(this.gameObject);
        Destroy(_collidedTrain);
        Time.timeScale = 1f;
    }
}
