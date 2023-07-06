using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _trainRigidbody;
    private TrainManager _trainMgr;

    private Coroutine _trainReplenishCoroutine;

    // Absolute values (direction independent)
    // TODO: Read from Train's attributes and make them private (once the save/load is properly implemented)
    // Exposed to be able to save to the backend
    // TODO: Encapsulate these into a struct for easier data passing to save.
    private float _acceleration = 1; 
    public float CurrentSpeed { get;  private set; }
    public float MaxSpeed { get; private set; }
    public TrainDirection MovementDirn { get; private set; }

    private CurveType _curveType;
    private TrainState _trainState;
    private List<Transform> _waypointPath;

    // The 4 kinds of curved tracks and the straights
    private enum CurveType
    {
        RightUp,
        RightDown,
        LeftUp,
        LeftDown,
        Straight
    }

    private enum TrainState
    {
        StationEnter,
        StationStopped,
        StationDeparted
    }

    /////////////////////////////////////////////////////////
    // INITIALISATION
    /////////////////////////////////////////////////////////

    private void Awake()
    {
        if (!_trainRigidbody) Debug.LogError("RigidBody not attached to train");
        _trainMgr = this.GetComponent<TrainManager>();
        MaxSpeed = 5;
    }

    void Update()
    {
        if (_trainState == TrainState.StationDeparted)
        {
            CurrentSpeed += _acceleration * Time.deltaTime;
        }
        if (CurrentSpeed > MaxSpeed)
        {
            CurrentSpeed = MaxSpeed;
        }

        // TODO: Make it update periodically rather than every frame. To do it alongside the save/load fix
        _trainMgr.SaveCurrentTrainStatus();
    }

    private IEnumerator TrainStationEnter(GameObject station)
    {
        _trainRigidbody.velocity = Vector3.zero; // Removes residual motion from staight-line movement.
        int i = 0;
        float decelerationStep = CurrentSpeed / _waypointPath.Count;
        Vector3 currentWaypointPos;

        // Slows to a stop via waypoints
        while (i < _waypointPath.Count && CurrentSpeed > 0)
        {

            if (MovementDirn == TrainDirection.EAST)
            {
                currentWaypointPos = _waypointPath[i].position;
            }
            else if (MovementDirn == TrainDirection.WEST)
            {
                currentWaypointPos = _waypointPath[_waypointPath.Count - i - 1].position;
            }
            else
            {
                Debug.LogError($"[TrainMovement] {this.name}: Train entering the station in the wrong orientation!");
                yield break;
            }

            this.transform.position = Vector3.MoveTowards(this.transform.position, currentWaypointPos, CurrentSpeed * Time.deltaTime);
            float difference = Vector3.Distance((Vector3)this.transform.position, currentWaypointPos);
            if (difference < 0.1f)
            {
                CurrentSpeed -= decelerationStep;
                i++;
            }
            yield return null;
        }

        if (CurrentSpeed < 0) CurrentSpeed = 0;
        _waypointPath = null;
        _trainState = TrainState.StationStopped;

        _trainMgr.StationEnterProcedure(station);
    }

    //////////////////////////////////////////////////////
    /// TRAIN MOVEMENT LOGIC (STATION_DEPART)
    //////////////////////////////////////////////////////

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Populate waypoints
            // 1. Curved tracks for moveRotate()
            // 2. Stations for the slowdown effect in trainStationEnter()
        // Else the waypoints will be an empty one
        _waypointPath = new List<Transform>();
        Transform[] children = other.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.CompareTag("TrackWaypoint") && child.IsChildOf(other.transform))
            {
                _waypointPath.Add(child);
            }
        }

        // Sets the relevant flags so that the MoveTrain function will know how to divert code execution
        switch (other.tag)
        {
            case "PlatformTD":
            case "PlatformLR":
                _trainState = TrainState.StationEnter;
                StartCoroutine(TrainStationEnter(other.gameObject));
                _trainReplenishCoroutine = StartCoroutine(_trainMgr.ReplenishTrainFuelAndDurability());
                break;
            case "Track_Curved_RU":
                _curveType = CurveType.RightUp;
                break;
            case "Track_Curved_RD":
                _curveType = CurveType.RightDown;
                break;
            case "Track_Curved_LU":
                _curveType = CurveType.LeftUp;
                break;
            case "Track_Curved_LD":
                _curveType = CurveType.LeftDown;
                break;

            case "Track_LR":
            case "Track_TD":
                _curveType = CurveType.Straight;
                break;
            default:
                Debug.LogError($"[TrainMovement] {this.name}: Invalid Tag in the Train's Trigger Zone");
                break;
        }
    }

    private IEnumerator MoveTrain()
    {
        while (_trainState == TrainState.StationDeparted)
        {
            switch (_curveType)
            {
                case CurveType.Straight:
                    MoveTrainStraight(MovementDirn);
                    break;
                case CurveType.RightUp:
                    yield return StartCoroutine(MoveTrainRightUp(MovementDirn));
                    break;
                case CurveType.RightDown:
                    yield return StartCoroutine(MoveTrainRightDown(MovementDirn));
                    break;
                case CurveType.LeftUp:
                    yield return StartCoroutine(MoveTrainLeftUp(MovementDirn));
                    break;
                case CurveType.LeftDown:
                    yield return StartCoroutine(MoveTrainLeftDown(MovementDirn));
                    break;
                default:
                    Debug.LogError("[TrainMovement] MoveTrain Switch Case Not Implemented Error");
                    yield break;
            }
            yield return null;
        }
    }


    private void MoveTrainStraight(TrainDirection currentDirn)
    {
        switch (currentDirn)
        {
            case TrainDirection.NORTH:
                _trainRigidbody.velocity = new Vector3(0, CurrentSpeed, 0);
                break;
            case TrainDirection.SOUTH:
                _trainRigidbody.velocity = new Vector3(0, -CurrentSpeed, 0);
                break;
            case TrainDirection.EAST:
                _trainRigidbody.velocity = new Vector3(CurrentSpeed, 0, 0);
                break;
            case TrainDirection.WEST:
                _trainRigidbody.velocity = new Vector3(-CurrentSpeed, 0, 0);
                break;
            default:
                Debug.LogError($"[TrainMovement] {this.name}: Invalid Direction being used to move in a straight line");
                break;
        }
    }

    private IEnumerator MoveTrainRightUp(TrainDirection currentDirn)
    {
        if (currentDirn == TrainDirection.EAST)
        {
            yield return StartCoroutine(MoveAndRotate(true));
            MovementDirn = TrainDirection.NORTH;

        } 
        else if (currentDirn == TrainDirection.SOUTH) 
        {
            yield return StartCoroutine(MoveAndRotate(false));
            MovementDirn = TrainDirection.WEST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }
        curveExitCheck();
    }

    private IEnumerator MoveTrainRightDown(TrainDirection currentDirn)
    {
        if (currentDirn == TrainDirection.EAST)
        {
            yield return StartCoroutine(MoveAndRotate(false));
            MovementDirn = TrainDirection.SOUTH;

        }
        else if (currentDirn == TrainDirection.NORTH)
        {
            yield return StartCoroutine(MoveAndRotate(true));
            MovementDirn = TrainDirection.WEST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }
        curveExitCheck();
    }

    private IEnumerator MoveTrainLeftUp(TrainDirection currentDirn)
    {
        if (currentDirn == TrainDirection.WEST)
        {
            yield return StartCoroutine(MoveAndRotate(false));
            MovementDirn = TrainDirection.NORTH;

        }
        else if (currentDirn == TrainDirection.SOUTH)
        {
            yield return StartCoroutine(MoveAndRotate(true));
            MovementDirn = TrainDirection.EAST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }
        curveExitCheck();
    }

    private IEnumerator MoveTrainLeftDown(TrainDirection currentDirn)
    {
        if (currentDirn == TrainDirection.WEST)
        {
            yield return StartCoroutine(MoveAndRotate(true));
            MovementDirn = TrainDirection.SOUTH;

        }
        else if (currentDirn == TrainDirection.NORTH)
        {
            yield return StartCoroutine(MoveAndRotate(false));
            MovementDirn = TrainDirection.EAST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }
        curveExitCheck();
    }


    private IEnumerator MoveAndRotate(bool rotateLeft)
    {        
        int i = 0;
        float degreesRotated = 0;
        float initialRotationAngle = _trainRigidbody.rotation;
        _trainRigidbody.velocity = Vector3.zero; // Removes the residual velocity that arises from moving straight, or it will cause a curved path between waypoints
        Vector3 currentWaypointPos;

        while (i < _waypointPath.Count)
        {
            if (degreesRotated > 90) degreesRotated = 90;

            if (rotateLeft)
            {
                _trainRigidbody.MoveRotation(initialRotationAngle + degreesRotated);
                currentWaypointPos = _waypointPath[i].position;
            }
            else
            {
                _trainRigidbody.MoveRotation(initialRotationAngle - degreesRotated);
                currentWaypointPos = _waypointPath[_waypointPath.Count - i -1].position;
            }
                
            this.transform.position = Vector3.MoveTowards(this.transform.position, currentWaypointPos, CurrentSpeed * Time.deltaTime );
            float difference = Vector3.Distance((Vector3)this.transform.position, currentWaypointPos);
            if (difference < 0.1f)
            {
                // Dirty fix to make the rotation look more correct
                if (degreesRotated == 0 && i !=0) degreesRotated += 5f;
                if (i != 0) degreesRotated += 5; 
                i++;
            }
            yield return null;
        }

        // Move and Rotation Finish Condition
        _waypointPath = null;
        _curveType = CurveType.Straight;
    }

    // Called after moveRotate has finished.
    private void curveExitCheck()
    {
        if (_curveType != CurveType.Straight)
        {
            Debug.LogError("moveRotate did not set the curve type from curved to straight");
        }
    }

    //////////////////////////////////////////////////////
    /// PUBLIC FUNCTIONS
    //////////////////////////////////////////////////////
    public void DepartTrain(bool isRight)
    {
        if (isRight) MovementDirn = TrainDirection.EAST;
        else MovementDirn = TrainDirection.WEST;

        _curveType = CurveType.Straight;
        _trainState = TrainState.StationDeparted;
        _trainMgr.StationExitProcedure(null);

        StartCoroutine(MoveTrain());
        StopCoroutine(_trainReplenishCoroutine);
    }
}
