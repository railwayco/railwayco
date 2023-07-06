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


    /////////////////////////////////////////
    // CAMERA VIEWPORT CHANGES
    ////////////////////////////////////////

    // Modifies the rect positions. Affect the viewportPoint values as clicks beyond the "valid" rect positions will return a >1
    public void RightPanelActiveCameraUpdate(float rightPanelWidthRatio, bool isTrainInStation)
    {
        float worldCamScreenHeightRatio = 0.3f;

        Camera worldCam = _worldCam.GetComponent<Camera>();

        if (isTrainInStation)
        {
            worldCam.rect = new Rect(0, 1 - worldCamScreenHeightRatio, 1 - rightPanelWidthRatio, worldCamScreenHeightRatio);

            _minimapCam.SetActive(true);
            Camera minimapCam = _minimapCam.GetComponent<Camera>();
            minimapCam.rect = new Rect(0, 0, 1 - rightPanelWidthRatio, 1 - worldCamScreenHeightRatio);
        } 
        else
        {
            worldCam.rect = new Rect(0, 0, 1 - rightPanelWidthRatio, 1f);
        }
        //TODO: Scale up the world cam
        // TODO: Set minimaap cam active
    }


    public void RightPanelInactiveCameraUpdate()
    {

        Camera worldCam = _worldCam.GetComponent<Camera>();
        worldCam.rect = new Rect(0, 0, 1f, 1f);

        _minimapCam.SetActive(false);
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
