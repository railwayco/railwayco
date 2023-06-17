using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCameraMovement : MonoBehaviour
{
    enum CameraMode
    {
        USER_DRAG,
        TRAIN_TRACKING,
        STATION_TRACKING
    }

    public Camera worldCam;
    CameraMode camMode = CameraMode.USER_DRAG;
    private float dragSpeed = 25f;
    private float zoomSpeed = 2f;
    private Vector3 dragOrigin; // In World Coordinates
    private GameObject objToFollow;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            camMode = CameraMode.USER_DRAG;
            dragOrigin = worldCam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (camMode == CameraMode.USER_DRAG)
        {
            moveMouse();
        }

        zoomFunction();
    }

    private void zoomFunction()
    {
        float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        worldCam.orthographicSize -= zoomAmount;
        if (worldCam.orthographicSize < 1)
        {
            worldCam.orthographicSize = 1;
        }
        if (worldCam.orthographicSize > 30)
        {
            worldCam.orthographicSize = 30;
        }
    }

    private void moveMouse()
    {

        if (!Input.GetMouseButton(0)) return;


        if (camMode == CameraMode.USER_DRAG)
        {
            Vector3 dragDelta = worldCam.ScreenToWorldPoint(Input.mousePosition) - dragOrigin; // World Coordinates
            transform.position -= dragDelta * dragSpeed * Time.deltaTime * worldCam.orthographicSize;
        }

    }

    public void followtrain(GameObject train)
    {
        camMode = CameraMode.TRAIN_TRACKING;
        objToFollow = train;
        StartCoroutine(cameraFollowTrain());
    }

    private IEnumerator cameraFollowTrain()
    {
        this.GetComponent<Camera>().orthographicSize = 5;
        while(camMode == CameraMode.TRAIN_TRACKING)
        {
            Vector3 trainPos = objToFollow.transform.position;
            transform.position = new Vector3(trainPos.x, trainPos.y, -10);
            yield return null;
        }
    }

    public void followStation(GameObject station)
    {
        camMode = CameraMode.STATION_TRACKING;

        this.GetComponent<Camera>().orthographicSize = 5;
        Vector3 stationPos= station.transform.position;
        transform.position = new Vector3(stationPos.x, stationPos.y, -10);
    }
}
