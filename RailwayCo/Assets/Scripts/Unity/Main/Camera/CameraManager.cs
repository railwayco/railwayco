using UnityEngine;


/// <summary>
/// To be attached to CameraList only. Code that require the set of functions shall query from CameraList.
/// </summary>
public class CameraManager : MonoBehaviour
{
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
        Transform worldCam = this.transform.Find("WorldCamera");
        if (!worldCam)
        {
            Debug.LogError("World Camera is not inside the Camera List!");
            _worldCam = null;
        }

        _worldCam = worldCam.gameObject;
        _worldCamScript = _worldCam.GetComponent<WorldCameraMovement>();
        if (!_worldCamScript)
        {
            Debug.LogError("There is no Camera Movement script attached to the WorldCamera!");
        }

        Transform minimapCam = this.transform.Find("MinimapCamera");
        if (!minimapCam)
        {
            Debug.LogError("Minimap Camera is not inside the camera List!");
            _minimapCam = null;
        }
        _minimapCam = minimapCam.gameObject;
        _minimapCamScript = _minimapCam.GetComponent<MinimapCameraMovement>();

    }

    private void Start()
    {
        DefaultCameraRendering();
    }

    public void SetBottomPanelHeightRatio(float bottomPanelHtRatio)
    {
        _uiBottomPanelHeightRatio = bottomPanelHtRatio;
    }

    /////////////////////////////////////////
    // CAMERA VIEWPORT CHANGES
    ////////////////////////////////////////
    private void DefaultCameraRendering()
    {
        _worldCam.GetComponent<Camera>().rect = new Rect(0, _uiBottomPanelHeightRatio, 1f, 1f);
        _minimapCam.SetActive(false);
    }

    // Modifies the rect positions. Affect the viewportPoint values as clicks beyond the "valid" rect positions will return a >1
    public void RightPanelActivateCameraUpdate(float rightPanelWidthRatio, bool isTrainInStation)
    {
        float worldCamScreenHeightRatio = 0.3f; // Ratio the world camera takes on the screen in the presense of a minimap
        _minimapCam.SetActive(false); // Reset the minimap camera

        Camera worldCam = _worldCam.GetComponent<Camera>();

        if (isTrainInStation)
        {
            worldCam.rect = new Rect(0, 1 - worldCamScreenHeightRatio, 1 - rightPanelWidthRatio, worldCamScreenHeightRatio);

            _minimapCam.SetActive(true);
            _minimapCam.GetComponent<Camera>().rect = new Rect(0, 0, 1 - rightPanelWidthRatio, 1 - worldCamScreenHeightRatio);
        } 
        else
        {
            worldCam.rect = new Rect(0, 0, 1 - rightPanelWidthRatio, 1f);
        }
    }


    public void RightPanelInactivateCameraUpdate()
    {
        DefaultCameraRendering();
    }


    /////////////////////////////////////////
    // WORLD CAMERA MANIPULATION
    ////////////////////////////////////////

    /// <summary>
    /// Primarily used by the UI_World Button
    /// </summary>
    /// <param name="setDefaultWorldCoords"> whether the set/get the coordinates of the default world position </param>
    public void SetDefaultWorldView(bool setDefaultWorldCoords)
    {
        if (setDefaultWorldCoords)
        {
            _defaultWorldPos = _worldCam.transform.position;
        }
        else
        {
            _worldCam.transform.position = _defaultWorldPos;
        }
    }

    public void WorldCamFollowTrain(GameObject trainToFollow)
    {
        _worldCamScript.Followtrain(trainToFollow);
    }

    public void WorldCamFollowStation(GameObject stationToFollow)
    {
        _worldCamScript.FollowStation(stationToFollow);
    }
}
