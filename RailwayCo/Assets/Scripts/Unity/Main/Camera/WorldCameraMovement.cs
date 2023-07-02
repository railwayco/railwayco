using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WorldCameraMovement : MonoBehaviour
{
    enum CameraMode
    {
        USER_DRAG,
        TRAIN_TRACKING,
        STATION_TRACKING
    }
    [SerializeField] private Camera _worldCam;
    private CameraMode _camMode = CameraMode.USER_DRAG;

    private readonly float _dragSpeed = 25f;
    private readonly float _zoomSpeed = 6f;

    private Vector3 _dragOrigin; // In World Coordinates

    private float _rightPanelWidthRatio;
    private GameObject _rightPanel;

    private void Awake()
    {
        if (!_worldCam) Debug.LogError("World Camera Ref is not set!");

        GameObject mainUI = GameObject.Find("MainUI");
        if (!mainUI) Debug.LogError("Main UI Not Found!");

        Vector2 refReso = mainUI.GetComponent<CanvasScaler>().referenceResolution;
        _rightPanel = mainUI.transform.Find("RightPanel").gameObject;
        if (!_rightPanel) Debug.LogError("Right Panel Object not found!");
        _rightPanelWidthRatio = _rightPanel.GetComponent<RectTransform>().rect.width/ refReso[0];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _rightPanel.SetActive(false);
        }
        CheckActiveSidePanel(_worldCam, _rightPanel, _rightPanelWidthRatio);

        if (Input.GetMouseButtonDown(0))
        {
            _camMode = CameraMode.USER_DRAG;
            _dragOrigin = _worldCam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (_camMode == CameraMode.USER_DRAG)
        {
            MoveMouse(_worldCam, _dragOrigin, _dragSpeed);
        }

        ZoomFunction(_worldCam, _zoomSpeed);
    }

    private void CheckActiveSidePanel(Camera worldCam, GameObject rightPanel, float rightPanelWidthRatio)
    {
        if (rightPanel.activeInHierarchy)
        {
            worldCam.rect = new Rect(worldCam.rect.x, worldCam.rect.y, 1- rightPanelWidthRatio, 1f);
        }
        else
        {
            worldCam.rect = new Rect(worldCam.rect.x, worldCam.rect.y, 1f, 1f);
        }
    }

    private void ZoomFunction(Camera worldCam, float zoomSpeed)
    {
        float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        worldCam.orthographicSize -= zoomAmount;
        if (worldCam.orthographicSize < 1)
        {
            worldCam.orthographicSize = 1;
        }
        else if (worldCam.orthographicSize > 30)
        {
            worldCam.orthographicSize = 30;
        }
    }

    private void MoveMouse(Camera worldCam, Vector3 dragOrigin, float dragSpeed)
    {

        if (!Input.GetMouseButton(0)) return;
        Vector2 viewPort = worldCam.ScreenToViewportPoint(Input.mousePosition);
        if (viewPort.x > 1 || viewPort.y > 1) return;


        if (_camMode == CameraMode.USER_DRAG)
        {
            Vector3 dragDelta = worldCam.ScreenToWorldPoint(Input.mousePosition) - dragOrigin; // World Coordinates
            Vector3 outcome = dragDelta * dragSpeed * Time.deltaTime * (10/ worldCam.orthographicSize);
            transform.position -= outcome;
        }

    }

    private IEnumerator CameraFollowTrain(GameObject train)
    {
        this.GetComponent<Camera>().orthographicSize = 7;
        while(_camMode == CameraMode.TRAIN_TRACKING)
        {
            Vector3 trainPos = train.transform.position;
            transform.position = new Vector3(trainPos.x, trainPos.y, -10);
            yield return null;
        }
    }

    /////////////////////////////////
    /// PUBLIC FUNCTIONS
    /////////////////////////////////
    public void Followtrain(GameObject train)
    {
        _camMode = CameraMode.TRAIN_TRACKING;
        StartCoroutine(CameraFollowTrain(train));
    }
    public void FollowStation(GameObject station)
    {
        _camMode = CameraMode.STATION_TRACKING;

        this.GetComponent<Camera>().orthographicSize = 7;
        Vector3 stationPos = station.transform.position;
        transform.position = new Vector3(stationPos.x, stationPos.y, -10);
    }
}
