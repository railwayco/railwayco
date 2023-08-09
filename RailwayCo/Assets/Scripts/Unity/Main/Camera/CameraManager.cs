using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager Instance { get; set; }

    private GameObject _worldCam;
    private WorldCameraMovement _worldCamScript;

    private GameObject _minimapCam;
    private MinimapCameraMovement _minimapCamScript;

    private Vector3 _defaultWorldPos;
    private float _uiBottomPanelHeightRatio;


    /////////////////////////////////////////
    // INITIALISATION
    ////////////////////////////////////////

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        Transform worldCam = transform.Find("WorldCamera");
        if (!worldCam)
        {
            Debug.LogError("World Camera is not inside the Camera List!");
            Instance._worldCam = null;
        }

        Instance._worldCam = worldCam.gameObject;
        Instance._worldCamScript = Instance._worldCam.GetComponent<WorldCameraMovement>();
        if (!Instance._worldCamScript)
        {
            Debug.LogError("There is no Camera Movement script attached to the WorldCamera!");
        }

        Transform minimapCam = transform.Find("MinimapCamera");
        if (!minimapCam)
        {
            Debug.LogError("Minimap Camera is not inside the camera List!");
            Instance._minimapCam = null;
        }
        Instance._minimapCam = minimapCam.gameObject;
        Instance._minimapCamScript = Instance._minimapCam.GetComponent<MinimapCameraMovement>();

    }

    private static void Start()
    {
        DefaultCameraRendering();
    }

    public static void SetBottomPanelHeightRatio(float bottomPanelHtRatio)
    {
        Instance._uiBottomPanelHeightRatio = bottomPanelHtRatio;
    }

    /////////////////////////////////////////
    // CAMERA VIEWPORT CHANGES
    ////////////////////////////////////////
    private static void DefaultCameraRendering()
    {
        Instance._worldCam.GetComponent<Camera>().rect = new Rect(0, Instance._uiBottomPanelHeightRatio, 1f, 1f);
        Instance._minimapCam.GetComponent<Camera>().rect = new Rect(0, Instance._uiBottomPanelHeightRatio, 1f, 1f);
        Instance._minimapCam.SetActive(false);
    }

    // Modifies the rect positions. Affect the viewportPoint values as clicks beyond the "valid" rect positions will return a >1
    public static void RightPanelActivateCameraUpdate(float rightPanelWidthRatio, bool isTrainInPlatform)
    {
        float worldCamScreenHeightRatio = 0.3f; // Ratio the world camera takes on the screen in the presense of a minimap
        Instance._minimapCam.SetActive(false); // Reset the minimap camera

        Camera worldCam = Instance._worldCam.GetComponent<Camera>();

        if (isTrainInPlatform)
        {
            worldCam.rect = new Rect(0, 1 - worldCamScreenHeightRatio, 1 - rightPanelWidthRatio, worldCamScreenHeightRatio);

            Instance._minimapCam.SetActive(true);
            Instance._minimapCam.GetComponent<Camera>().rect = new Rect(0, 0, 1 - rightPanelWidthRatio, 1 - worldCamScreenHeightRatio);
        }
        else
            worldCam.rect = new Rect(0, 0, 1 - rightPanelWidthRatio, 1f);
    }

    public static void RightPanelInactivateCameraUpdate() => DefaultCameraRendering();

    public static string ToggleWorldMinimapCamera(string UiText)
    {
        // Close the right panel if it is enabled
        RightPanelManager.CloseRightPanel();

        if (UiText == "World")
        {
            Debug.Log("WORLD");
            Instance._worldCam.SetActive(true);
            Instance._minimapCam.SetActive(false);
            return "Minimap";
        }
        else if (UiText == "Minimap")
        {
            Debug.Log("MINIMAP");
            Instance._minimapCam.SetActive(true);
            Instance._worldCam.SetActive(false);
            return "World";
        }
        else
        {
            Debug.LogWarning("Unkonwn UI Text being passed");
            return UiText;
        }
    }

    /////////////////////////////////////////
    // WORLD CAMERA MANIPULATION
    ////////////////////////////////////////

    /// <summary>
    /// Primarily used by the UI_World Button
    /// </summary>
    /// <param name="setDefaultWorldCoords"> whether the set/get the coordinates of the default world position </param>
    public static void SetDefaultWorldView(bool setDefaultWorldCoords)
    {
        if (setDefaultWorldCoords)
            Instance._defaultWorldPos = Instance._worldCam.transform.position;
        else
            Instance._worldCam.transform.position = Instance._defaultWorldPos;
    }

    public static void WorldCamFollowTrain(Guid trainGuid)
    {
        GameObject trainToFollow = TrainManager.GetGameObject(trainGuid);
        Instance._worldCamScript.Followtrain(trainToFollow);
    }

    public static void WorldCamFollowPlatform(Guid platformGuid)
    {
        GameObject platformToFollow = PlatformManager.GetGameObject(platformGuid);
        Instance._worldCamScript.FollowPlatform(platformToFollow);
    }
}
