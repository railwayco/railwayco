using System;
using System.Collections;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    public Guid TrainGuid { get; private set; } // Exposed to uniquely identify the train
    private TrainMovement _trainMovement;
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

        TrainGuid = TrainManager.GetTrainClassObject(gameObject.transform.position).Guid;
    }

    private void Start()
    {
        _trainMovement = gameObject.GetComponent<TrainMovement>();
        StartCoroutine(SaveCurrentTrainStatus());
    }

    private void OnMouseUpAsButton()
    {
        LoadCargoPanelViaTrain();
        FollowTrain();
    }

    private void UpdateAssocPlatform(GameObject platform)
    {
        if (_assocPlatform && platform) 
            Debug.LogWarning("This scenario should not happen! Will take the passed in parameter");

        if (platform)
        {
            PlatformController platformCtr = platform.GetComponent<PlatformController>();
            platformCtr.UpdateAssocTrain(gameObject);
        }
        else if (_assocPlatform)
        {
            PlatformController platformCtr = _assocPlatform.GetComponent<PlatformController>();
            platformCtr.UpdateAssocTrain(null);
        }
        else
        {
            Debug.LogError("This path should not happen! Either platform or _assocPlatform must be non-null!");
            return;
        }

        _assocPlatform = platform;
    }

    //////////////////////////////////////////////////////
    // PUBLIC FUNCTIONS
    //////////////////////////////////////////////////////

    public void PlatformEnterProcedure(GameObject platform)
    {
        UpdateAssocPlatform(platform);
        Guid stationGuid = platform.GetComponent<PlatformController>().StationGuid;
        CargoManager.ProcessTrainCargo(TrainGuid, stationGuid);

        // Will want to update the TrainOnly panel (and incidentally, StationOnly panel) to TrainStationPanel automatically
        // once the train has docked at the platform (and keep accurate information)
        if (RightPanelManager.IsActiveAndEnabled)
        {
            if (!RightPanelManager.IsActivePanelSamePanelType(RightPanelType.Cargo))
                return;

            if (!RightPanelManager.IsActiveCargoPanelSameTrainOrPlatform(gameObject, platform))
                return;

            RightPanelManager.LoadCargoPanel(gameObject, platform, CargoTabOptions.TrainCargo);
        }
    }

    public void PlatformExitProcedure() => UpdateAssocPlatform(null);

    public IEnumerator SaveCurrentTrainStatus()
    {
        while (true)
        {
            TrainManager.UpdateTrainBackend(_trainMovement.TrainAttribute, TrainGuid);
            yield return new WaitForSecondsRealtime(5);
        }
    }

    public TrainAttribute GetTrainAttribute() => TrainManager.GetTrainAttribute(TrainGuid);

    public bool RepairTrain(CurrencyManager cost) => TrainManager.RepairTrain(TrainGuid, cost);

    public IEnumerator RefuelTrain()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(30);
            TrainManager.RefuelTrain(TrainGuid);
        }
    }

    public void LoadCargoPanelViaTrain()
    {
        RightPanelManager.LoadCargoPanel(gameObject, _assocPlatform, CargoTabOptions.Nil);
    }

    public void FollowTrain() => CameraManager.WorldCamFollowTrain(gameObject);

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
        TrainManager.OnTrainCollision(TrainGuid);
        TrainManager.OnTrainCollision(_collidedTrain.GetComponent<TrainController>().TrainGuid);

        _collisionPanel.SetActive(false);
        Destroy(gameObject);
        Destroy(_collidedTrain);
        Time.timeScale = 1f;
    }
}
