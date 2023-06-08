using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCamera : MonoBehaviour

{
    /*
     cameraMode 0 : User Drag around
    cameraMode 1: Track the moving train
     */
    public Camera WorldCam;
    private int cameraMode = 0;
    private Vector3 dragOrigin;
    public float dragSpeed = 5f;
    public float zoomSpeed = 1f;
    private GameObject trainToFollow;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            cameraMode = 0;
            dragOrigin = Input.mousePosition;
        }
        if (cameraMode == 0)
        {
            moveMouse();

        }

        else if (cameraMode == 1)
        {
            cameraFollowTrain();
        }

        zoomFunction();
    }

    private void zoomFunction()
    {
        float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        WorldCam.orthographicSize -= zoomAmount;
        if (WorldCam.orthographicSize < 1)
        {
            WorldCam.orthographicSize = 1;
        }


    }

    private void moveMouse()
    {

        // TO TOUCH UP: Trying to drag around when zoomed in results in a hypersensitive movement. 
        // Suggest to fix this in the next iteration
        if (!Input.GetMouseButton(0)) return;

        //if (Input.GetMouseButtonDown(0))
        //{
        //    cameraMode = 0;
        //    dragOrigin = Input.mousePosition;
        //    return;
        //}

        if (cameraMode == 0)
        {
        Vector3 dragDelta = Input.mousePosition - dragOrigin;
        Vector3 newPosition = transform.position - dragDelta * dragSpeed * Time.deltaTime;
        transform.position = newPosition;
        dragOrigin = Input.mousePosition;
            Debug.Log(transform.position);

        }

    }
    public IEnumerator followtrain (GameObject train)
    {
        yield return null;
        Debug.Log("This is activated");
        cameraMode =1;
        trainToFollow = train;
    }

    private void cameraFollowTrain()
    {
        Vector3 trainPos = trainToFollow.transform.position;
        transform.position = new Vector3 (trainPos.x, trainPos.y, -10);
        Debug.Log(transform.position);
    }
}
