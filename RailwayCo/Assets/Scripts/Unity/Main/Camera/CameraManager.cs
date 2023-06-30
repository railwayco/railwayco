using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// To be attached to CameraList only. Code that require the set of functions shall query from CameraList.
/// </summary>
public class CameraManager : MonoBehaviour
{
    private WorldCameraMovement _worldCamScript;
    private GameObject _worldCam;
    private Vector3 _defaultWorldPos;

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
    }

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
