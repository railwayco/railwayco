using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _trainRigidbody;
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

    private TrackType _trackType;
    private TrainState _trainState;
    private List<Transform> _waypointPath;

    // The 4 kinds of curved tracks and the straights
    private enum TrackType
    {
        RightUp,
        RightDown,
        LeftUp,
        LeftDown,
        StraightGround,
        StraightBridge,
        InclineUp,
        InclineDown
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

            if (MovementDirn == TrainDirection.EAST || MovementDirn == TrainDirection.SOUTH)
            {
                currentWaypointPos = _waypointPath[i].position;
            }
            else if (MovementDirn == TrainDirection.WEST || MovementDirn == TrainDirection.NORTH)
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
    /// TRAIN MOVEMENT DETERMINATION LOGIC (STATION_DEPART-ed)
    //////////////////////////////////////////////////////

    private void OnTriggerEnter(Collider other)
    {
        // Due to the introduction of 2 Box colliders for the curved trakcks,
        // we need to check for and ignore the second box collider and not proceed with further processing.
        if (IsStillCurvedTrack(other.tag, _trackType)) return;

        _waypointPath = GetWaypoints(other);
        
        Debug.LogError(other.tag);
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
                _trackType = TrackType.RightUp;
                break;
            case "Track_Curved_RD":
                _trackType = TrackType.RightDown;
                break;
            case "Track_Curved_LU":
                _trackType = TrackType.LeftUp;
                break;
            case "Track_Curved_LD":
                _trackType = TrackType.LeftDown;
                break;
            case "Track_LR":
            case "Track_TD":
                _trackType = TrackType.StraightGround;
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
            switch (_trackType)
            {
                case TrackType.StraightGround:
                    MoveTrainStraight(MovementDirn);
                    break;
                case TrackType.RightUp:
                    yield return StartCoroutine(MoveTrainRightUp(MovementDirn));
                    break;
                case TrackType.RightDown:
                    yield return StartCoroutine(MoveTrainRightDown(MovementDirn));
                    break;
                case TrackType.LeftUp:
                    yield return StartCoroutine(MoveTrainLeftUp(MovementDirn));
                    break;
                case TrackType.LeftDown:
                    yield return StartCoroutine(MoveTrainLeftDown(MovementDirn));
                    break;
                default:
                    Debug.LogError("[TrainMovement] MoveTrain Switch Case Not Implemented Error");
                    yield break;
            }
            yield return null;
        }
    }

    private List<Transform> GetWaypoints(Collider collided)
    {
        // Populate waypoints
        // 1. Curved tracks for moveRotate()
        // 2. Stations for the slowdown effect in trainStationEnter()
        // Else the waypoints will be an empty one
        // The waypoint generation is shifted here due to complications of 2 box colliders in the curved track (that also have the waypoint system)
        // We do not want the waypoint to reset itself when it reaches the 2nd box collider in the curved track so the check has to be done beforehand.
        List<Transform> waypoints = new List<Transform>();

        Transform[] children;
        if (collided.tag.Contains("Track_Curved_"))
        {
            // This is due to the fact that the box colliders in the curved track is not a component of the track itself
            // Rather, it is implemented as a child of the track
            children = collided.transform.parent.GetComponentsInChildren<Transform>();
        }
        else
        {
            children = collided.GetComponentsInChildren<Transform>();
        }


        foreach (Transform child in children)
        {
            if (child.CompareTag("TrackWaypoint"))
            {
                waypoints.Add(child);
            }
        }
        return waypoints;
    }

    private bool IsStillCurvedTrack(string collidedTagName, TrackType currentTrackType)
    {
        switch (collidedTagName)
        {
            case "Track_Curved_RU":
                if (currentTrackType == TrackType.RightUp) return true;
                break;
            case "Track_Curved_RD":
                if (currentTrackType == TrackType.RightDown) return true;
                break;
            case "Track_Curved_LU":
                if (currentTrackType == TrackType.LeftUp) return true; 
                break;
            case "Track_Curved_LD":
                if (currentTrackType == TrackType.LeftDown) return true;
                break;
            default:
                return false;
        }
        return false;
    }

    //////////////////////////////////////////////////////
    /// STRAIGHT MOVEMENT LOGIC
    //////////////////////////////////////////////////////

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



    //////////////////////////////////////////////////////
    /// CURVE MOVEMENT LOGIC
    //////////////////////////////////////////////////////

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
        CurveExitCheck();
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
        CurveExitCheck();
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
        CurveExitCheck();
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
        CurveExitCheck();
    }


    private IEnumerator MoveAndRotate(bool rotateLeft)
    {        
        int i = 0;
        float degreesRotated = 0;
        float initialRotationAngle = _trainRigidbody.rotation.eulerAngles.z;
        _trainRigidbody.velocity = Vector3.zero; // Removes the residual velocity that arises from moving straight, or it will cause a curved path between waypoints
        Vector3 currentWaypointPos;

        while (i < _waypointPath.Count)
        {
            if (degreesRotated > 90) degreesRotated = 90;

            if (rotateLeft)
            {
                _trainRigidbody.transform.rotation = Quaternion.Euler(_trainRigidbody.transform.eulerAngles.x,
                                                                        _trainRigidbody.transform.eulerAngles.y,
                                                                        initialRotationAngle + degreesRotated 
                                                                        );
                currentWaypointPos = _waypointPath[i].position;
            }
            else
            {
                _trainRigidbody.transform.rotation = Quaternion.Euler(_trainRigidbody.transform.eulerAngles.x,
                                                                        _trainRigidbody.transform.eulerAngles.y,
                                                                        initialRotationAngle - degreesRotated
                                                                        );
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
        _trackType = TrackType.StraightGround;
    }

    // Called after moveRotate has finished.
    private void CurveExitCheck()
    {
        if (_trackType != TrackType.StraightGround)
        {
            Debug.LogError("moveRotate did not set the curve type from curved to straight");
        }
    }

    //////////////////////////////////////////////////////
    /// PUBLIC FUNCTIONS
    //////////////////////////////////////////////////////
    public void DepartTrain(TrainDirection movementDirn)
    {
        MovementDirn = movementDirn;

        _trackType = TrackType.StraightGround;
        _trainState = TrainState.StationDeparted;
        _trainMgr.StationExitProcedure(null);

        StartCoroutine(MoveTrain());
        StopCoroutine(_trainReplenishCoroutine);
    }
}
