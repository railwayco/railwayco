using System;
using System.Collections;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    public Guid TrainGuid { get; private set; } // Exposed to uniquely identify the train
    private TrainMovement _trainMovement;
    private GameObject _assocPlatform;
    private Coroutine _trainRefuelCoroutine;

    /////////////////////////////////////
    /// INITIALISATION
    /////////////////////////////////////
    private void Awake() => TrainGuid = TrainManager.GetTrainClassObject(gameObject.transform.position).Guid;

    private void Start()
    {
        _trainMovement = gameObject.GetComponent<TrainMovement>();
        _trainMovement.EnterPlatform += TrainMovement_EnterPlatform;
        _trainMovement.ExitPlatform += TrainMovement_ExitPlatform;
        _trainMovement.StartRefuelTrain += TrainMovement_StartRefuelTrain;
        _trainMovement.StopRefuelTrain += TrainMovement_StopRefuelTrain;
        _trainMovement.TrainCollision += TrainMovement_TrainCollision;

        StartCoroutine(SaveTrainStatus());
    }

    private void TrainMovement_EnterPlatform(object sender, GameObject platform)
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

    private void TrainMovement_ExitPlatform(object sender, EventArgs e) => UpdateAssocPlatform(null);

    private void TrainMovement_StartRefuelTrain(object sender, EventArgs e) => _trainRefuelCoroutine = StartCoroutine(RefuelTrain());

    private void TrainMovement_StopRefuelTrain(object sender, EventArgs e) => StopCoroutine(_trainRefuelCoroutine);

    private void TrainMovement_TrainCollision(object sender, GameObject collidedTrain)
    {
        GameManager.ActivateCollisionPopup(gameObject, collidedTrain);
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

    private IEnumerator SaveTrainStatus()
    {
        for (; ; )
        {
            TrainManager.UpdateTrainBackend(TrainGuid, _trainMovement.TrainAttribute);
            yield return new WaitForSecondsRealtime(5);
        }
    }

    private IEnumerator RefuelTrain()
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
        TrainController collidedTrainCtr = otherTrain.GetComponent<TrainController>();
        GameManager.ActivateCollisionPopup(this, collidedTrainCtr);
    }
}
