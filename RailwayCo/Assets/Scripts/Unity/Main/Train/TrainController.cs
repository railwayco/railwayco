using System;
using System.Collections;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    public Guid TrainGuid { get; private set; } // Exposed to uniquely identify the train
    public Guid AssocPlatformGuid { get; private set; }
    private TrainMovement _trainMovement;
    private Coroutine _trainRefuelCoroutine;

    /////////////////////////////////////
    /// INITIALISATION
    /////////////////////////////////////
    private void Awake()
    {
        TrainGuid = TrainManager.GetTrainClassObject(gameObject.transform.position).Guid;

        _trainMovement = gameObject.GetComponent<TrainMovement>();
        _trainMovement.EnterPlatform += TrainMovement_EnterPlatform;
        _trainMovement.ExitPlatform += TrainMovement_ExitPlatform;
        _trainMovement.StartRefuelTrain += TrainMovement_StartRefuelTrain;
        _trainMovement.StopRefuelTrain += TrainMovement_StopRefuelTrain;
        _trainMovement.TrainCollision += TrainMovement_TrainCollision;
    }

    private void Start() => StartCoroutine(SaveTrainStatus());

    private void TrainMovement_EnterPlatform(object sender, Tuple<Guid, Guid> stationPlatformGuid)
    {
        Guid stationGuid = stationPlatformGuid.Item1;
        Guid platformGuid = stationPlatformGuid.Item2;

        UpdateAssocPlatform(platformGuid);
        CargoManager.ProcessTrainCargo(TrainGuid, stationGuid);

        // Will want to update the TrainOnly panel (and incidentally, StationOnly panel) to TrainStationPanel automatically
        // once the train has docked at the platform (and keep accurate information)
        if (RightPanelManager.IsActiveAndEnabled)
        {
            if (!RightPanelManager.IsActivePanelSamePanelType(RightPanelType.Cargo))
                return;

            if (!RightPanelManager.IsActiveCargoPanelSameTrainOrPlatform(TrainGuid, stationGuid))
                return;

            RightPanelManager.LoadCargoPanel(TrainGuid, platformGuid, CargoTabOptions.TrainCargo);
        }
    }

    private void TrainMovement_ExitPlatform(object sender, EventArgs e) => UpdateAssocPlatform(default);

    private void TrainMovement_StartRefuelTrain(object sender, EventArgs e) => _trainRefuelCoroutine = StartCoroutine(RefuelTrain());

    private void TrainMovement_StopRefuelTrain(object sender, EventArgs e) => StopCoroutine(_trainRefuelCoroutine);

    private void TrainMovement_TrainCollision(object sender, GameObject collidedTrain)
    {
        GameManager.ActivateCollisionPopup(gameObject, collidedTrain);
    }

    private void OnMouseUpAsButton()
    {
        RightPanelManager.LoadCargoPanel(TrainGuid, AssocPlatformGuid, CargoTabOptions.Nil);
        CameraManager.WorldCamFollowTrain(TrainGuid);
    }

    private void UpdateAssocPlatform(Guid platformGuid)
    {
        GameObject platform = PlatformManager.GetGameObject(platformGuid);
        GameObject assocPlatform = PlatformManager.GetGameObject(AssocPlatformGuid);
        if (assocPlatform && platform) 
            Debug.LogWarning("This scenario should not happen! Will take the passed in parameter");

        if (platform)
        {
            PlatformController platformCtr = platform.GetComponent<PlatformController>();
            platformCtr.UpdateAssocTrain(TrainGuid);
        }
        else if (assocPlatform)
        {
            PlatformController platformCtr = assocPlatform.GetComponent<PlatformController>();
            platformCtr.UpdateAssocTrain(default);
        }
        else
        {
            Debug.LogError("This path should not happen! Either platform or _assocPlatform must be non-null!");
            return;
        }

        AssocPlatformGuid = platformGuid;
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
}
