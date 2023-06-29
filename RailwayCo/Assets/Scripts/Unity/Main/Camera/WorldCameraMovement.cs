using System.Collections;
using System.Collections.Generic;
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
    private float _dragSpeed = 25f;
    private float _zoomSpeed = 2f;

    private Vector3 _dragOrigin; // In World Coordinates
    private GameObject _objToFollow;

    private float _rightPanelWidthRatio;
    private GameObject _rightPanel;
    private void Start()
    {
        GameObject mainUI = GameObject.Find("MainUI");
        Vector2 refReso = mainUI.GetComponent<CanvasScaler>().referenceResolution;
        _rightPanel = mainUI.transform.Find("RightPanel").gameObject;
        _rightPanelWidthRatio = _rightPanel.GetComponent<RectTransform>().rect.width/ refReso[0];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _rightPanel.SetActive(false);
        }
        CheckActiveSidePanel();
        if (Input.GetMouseButtonDown(0))
        {
            _camMode = CameraMode.USER_DRAG;
            _dragOrigin = _worldCam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (_camMode == CameraMode.USER_DRAG)
        {
            MoveMouse();
        }

        ZoomFunction();
    }

    private void CheckActiveSidePanel()
    {
        if (_rightPanel.activeInHierarchy)
        {
            _worldCam.rect = new Rect(_worldCam.rect.x, _worldCam.rect.y, 1- _rightPanelWidthRatio, 1f);
        }
        else
        {
            _worldCam.rect = new Rect(_worldCam.rect.x, _worldCam.rect.y, 1f, 1f);
        }
    }

    private void ZoomFunction()
    {
        float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed;
        _worldCam.orthographicSize -= zoomAmount;
        if (_worldCam.orthographicSize < 1)
        {
            _worldCam.orthographicSize = 1;
        }
        if (_worldCam.orthographicSize > 30)
        {
            _worldCam.orthographicSize = 30;
        }
    }

    private void MoveMouse()
    {

        if (!Input.GetMouseButton(0)) return;
        Vector2 viewPort = _worldCam.ScreenToViewportPoint(Input.mousePosition);
        if (viewPort.x > 1 || viewPort.y > 1) return;


        if (_camMode == CameraMode.USER_DRAG)
        {
            Vector3 dragDelta = _worldCam.ScreenToWorldPoint(Input.mousePosition) - _dragOrigin; // World Coordinates
            Vector3 outcome = dragDelta * _dragSpeed * Time.deltaTime * (10/ _worldCam.orthographicSize);
            transform.position -= outcome;
        }

    }

    public void Followtrain(GameObject train)
    {
        _camMode = CameraMode.TRAIN_TRACKING;
        _objToFollow = train;
        StartCoroutine(CameraFollowTrain());
    }

    private IEnumerator CameraFollowTrain()
    {
        this.GetComponent<Camera>().orthographicSize = 5;
        while(_camMode == CameraMode.TRAIN_TRACKING)
        {
            Vector3 trainPos = _objToFollow.transform.position;
            transform.position = new Vector3(trainPos.x, trainPos.y, -10);
            yield return null;
        }
    }

    public void FollowStation(GameObject station)
    {
        _camMode = CameraMode.STATION_TRACKING;

        this.GetComponent<Camera>().orthographicSize = 5;
        Vector3 stationPos= station.transform.position;
        transform.position = new Vector3(stationPos.x, stationPos.y, -10);
    }
}
