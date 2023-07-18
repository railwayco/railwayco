using UnityEngine;

public class MinimapCameraMovement : MonoBehaviour
{
    [SerializeField] private Camera _minimapCam;
    private readonly float _dragSpeed = 50f;
    private readonly float _zoomSpeed = 15f;
    private Vector3 _dragOrigin; // In World Coordinates

    private void Awake()
    {
        if (!_minimapCam) Debug.LogError("Minimap Camera Ref is not set!");
    }


    private void Update()
    {
        Vector2 viewPort = _minimapCam.ScreenToViewportPoint(Input.mousePosition);
        if (viewPort.x > 1 || viewPort.y > 1) return;
        if (viewPort.x < 0 || viewPort.y < 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            _dragOrigin = _minimapCam.ScreenToWorldPoint(Input.mousePosition);
        }

        MoveMouse(_minimapCam, _dragOrigin, _dragSpeed);
        ZoomFunction(_minimapCam, _zoomSpeed);
    }


    private void ZoomFunction(Camera minimapCam, float zoomSpeed)
    {
        float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        minimapCam.orthographicSize -= zoomAmount;
        if (minimapCam.orthographicSize < 30)
        {
            minimapCam.orthographicSize = 30;
        }
        else if (minimapCam.orthographicSize > 200)
        {
            minimapCam.orthographicSize = 200;
        }
    }

    private void MoveMouse(Camera minimapCam, Vector3 dragOrigin, float dragSpeed)
    {
        if (!Input.GetMouseButton(0)) return;

        Vector3 dragDelta = minimapCam.ScreenToWorldPoint(Input.mousePosition) - dragOrigin; // World Coordinates
        Vector3 outcome = dragDelta * dragSpeed * Time.deltaTime * (10 / minimapCam.orthographicSize);
        transform.position -= outcome;
    }
}
