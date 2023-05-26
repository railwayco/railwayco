using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCamera : MonoBehaviour

{
    
    public Camera WorldCam;
    private Vector3 dragOrigin;
    public float dragSpeed = 5f;
    public float zoomSpeed = 1f;

    // Update is called once per frame
    void Update()
    {

        moveMouse();
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

        Debug.Log($"{zoomAmount}, {WorldCam.orthographicSize}");

    }

    private void moveMouse()
    {

        // TO TOUCH UP: Trying to drag around when zoomed in results in a hypersensitive movement. 
        // Suggest to fix this in the next iteration
        if (!Input.GetMouseButton(0)) return;

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        Vector3 dragDelta = Input.mousePosition - dragOrigin;
        Vector3 newPosition = transform.position - dragDelta * dragSpeed * Time.deltaTime;
        transform.position = newPosition;
        dragOrigin = Input.mousePosition;
    }
}
