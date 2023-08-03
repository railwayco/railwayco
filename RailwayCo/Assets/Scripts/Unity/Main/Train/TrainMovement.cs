using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _trainRigidbody;
    private TrainManager _trainMgr;

    private Coroutine _trainRefuelCoroutine;

    public TrainAttribute TrainAttribute { get; private set; }
    private MovementDirection _movementDirection;
    private MovementState _movementState;
    private float _speed;

    // Absolute values (direction independent)
    private float _acceleration = 3;
    private float CurrentSpeed
    { 
        get => _speed;
        set
        {
            _speed = value;
            UpdateTrainAttribute();
        }
    }
    private float MaxSpeed => (float)TrainAttribute.Speed.UpperLimit;
    private MovementDirection MovementDirn
    {
        get => _movementDirection;
        set
        {
            _movementDirection = value;
            UpdateTrainAttribute();
        }
    }
    private MovementState MovementState 
    { 
        get => _movementState;
        set 
        {
            _movementState = value;
            UpdateTrainAttribute();
        } 
    }

    private TrackType _trackType = TrackType.Nil;
    private TrackType _prevTrackType;
    private List<Transform> _waypointPath;
    private Collider _collidedObject;

    // The 4 kinds of curved tracks and the straights
    private enum TrackType
    {
        Nil,
        RightUp,
        RightDown,
        LeftUp,
        LeftDown,
        StraightGround,
        StraightBridge,
        InclineUp,
        InclineDown
    }

    /////////////////////////////////////////////////////////
    // INITIALISATION
    /////////////////////////////////////////////////////////

    private void Awake()
    {
        if (!_trainRigidbody) Debug.LogError("RigidBody not attached to train");
        _trainMgr = this.GetComponent<TrainManager>();
    }

    private void OnEnable()
    {
        TrainAttribute = _trainMgr.GetTrainAttribute();
        _movementDirection = TrainAttribute.MovementDirection;
        _movementState = TrainAttribute.MovementState;
        StartCoroutine(LoadTrainStartMovement());
    }

    void Update()
    {
        if (MovementState == MovementState.Moving)
        {
            CurrentSpeed += _acceleration * Time.deltaTime;
        }
        if (CurrentSpeed > MaxSpeed)
        {
            CurrentSpeed = MaxSpeed;
        }
    }

    private IEnumerator TrainPlatformEnter(GameObject platform)
    {
        _trainRigidbody.velocity = Vector3.zero; // Removes residual motion from staight-line movement.
        int i = 0;
        float decelerationStep = CurrentSpeed / _waypointPath.Count;
        Vector3 currentWaypointPos;

        // Slows to a stop via waypoints
        while (i < _waypointPath.Count && CurrentSpeed > 0)
        {

            if (MovementDirn == MovementDirection.East || MovementDirn == MovementDirection.South)
            {
                currentWaypointPos = _waypointPath[i].position;
            }
            else if (MovementDirn == MovementDirection.West || MovementDirn == MovementDirection.North)
            {
                currentWaypointPos = _waypointPath[_waypointPath.Count - i - 1].position;
            }
            else
            {
                Debug.LogError($"[TrainMovement] {this.name}: Train entering the platform in the wrong orientation!");
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
        MovementState = MovementState.Stationary;

        _trainMgr.PlatformEnterProcedure(platform);
    }

    /////////////////////////////////////////////////////////
    /// TRAIN MOVEMENT DETERMINATION LOGIC (Platform-Depart)
    /////////////////////////////////////////////////////////

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Train"))
        {
            _trainMgr.TrainCollisionCleanupInitiate(collision.gameObject);
        }
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        // Due to the introduction of 2 Box colliders for the curved trakcks,
        // we need to check for and ignore the second box collider and not proceed with further processing.
        _collidedObject = other;
        if (IsStillCurvedTrack(other.tag, _trackType)) yield break;

        _waypointPath = GetWaypoints(other);
        
        _prevTrackType = _trackType;

        // Sets the relevant flags so that the MoveTrain function will know how to divert code execution
        switch (other.tag)
        {
            case "PlatformTD":
            case "PlatformLR":
                MovementState = MovementState.Stationary;
                CheckInclineAndSetRotation(TrackType.StraightGround);
                StartCoroutine(TrainPlatformEnter(other.gameObject));
                _trainRefuelCoroutine = StartCoroutine(_trainMgr.RefuelTrain());
                break;
            case "Track_Curved_RU":
                yield return CheckInclineAndSetRotation(TrackType.RightUp);
                _trackType = TrackType.RightUp;
                break;
            case "Track_Curved_RD":
                yield return CheckInclineAndSetRotation(TrackType.RightDown);
                _trackType = TrackType.RightDown;
                break;
            case "Track_Curved_LU":
                yield return CheckInclineAndSetRotation(TrackType.LeftUp);
                _trackType = TrackType.LeftUp;
                break;
            case "Track_Curved_LD":
                yield return CheckInclineAndSetRotation(TrackType.LeftDown);
                _trackType = TrackType.LeftDown;
                break;
            case "Track_LR":
            case "Track_TD":
                yield return CheckInclineAndSetRotation(TrackType.StraightGround);
                _trackType = TrackType.StraightGround;
                break;
            case "BridgeLR":
            case "BridgeTD":
                yield return CheckInclineAndSetRotation(TrackType.StraightBridge);
                _trackType = TrackType.StraightBridge;
                break;
            case "SlopeLR":
            case "SlopeTD":
                if (_prevTrackType == TrackType.StraightBridge)
                {
                    yield return CheckInclineAndSetRotation(TrackType.InclineDown);
                    _trackType = TrackType.InclineDown;
                }
                else if (_prevTrackType == TrackType.InclineUp || _prevTrackType == TrackType.InclineDown)
                {
                    break;
                }
                else
                {
                    yield return CheckInclineAndSetRotation(TrackType.InclineUp);
                    _trackType = TrackType.InclineUp;
                }
                break;
            default:
                Debug.LogError($"[TrainMovement] {this.name}: Invalid Tag in the Train's Trigger Zone");
                break;
        }
    }

    private IEnumerator MoveTrain()
    {
        while (MovementState == MovementState.Moving)
        {

            if (_trackType == TrackType.InclineUp || _trackType == TrackType.InclineDown)
            {
                
            }
            else if (_trackType == TrackType.StraightBridge)
            {
                //CheckInclineAndSetRotation();
                _trainRigidbody.position = new Vector3(_trainRigidbody.position.x,
                                                        _trainRigidbody.position.y,
                                                        -4
                                                        );
            }
            else
            {
                //CheckInclineAndSetRotation();
                _trainRigidbody.position = new Vector3(_trainRigidbody.position.x,
                                                        _trainRigidbody.position.y,
                                                        -1
                                                        );
            }

            switch (_trackType)
            {
                case TrackType.StraightGround:
                case TrackType.StraightBridge:
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
                case TrackType.InclineUp:
                case TrackType.InclineDown:
                    MoveTrainIncline(MovementDirn, _trackType);
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
        // 2. Platforms for the slowdown effect in trainPlatformEnter()
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

    // Checks if there is an incline change
    // If there is, move abit more and then set the rotation of the train
    // Else just return.
    private IEnumerator CheckInclineAndSetRotation(TrackType incomingTrack)
    {
        if (!CheckInclineChange(_prevTrackType, incomingTrack)) yield break;
        yield return MoveFixedDistance();
        
        
        int tiltAngle = 45;
        if (incomingTrack == TrackType.InclineDown)
        {
            tiltAngle *= -1;
        }
        else if (incomingTrack == TrackType.InclineUp)
        {
            tiltAngle *= 1;
        } 
        else
        {
            tiltAngle = 0;
        }



        if (MovementDirn == MovementDirection.East || MovementDirn == MovementDirection.South)
        {
            tiltAngle *= 1;
        } 
        else if (MovementDirn == MovementDirection.West || MovementDirn == MovementDirection.North)
        {
            tiltAngle *= -1;
        }




        if (incomingTrack == TrackType.InclineDown || incomingTrack == TrackType.InclineUp)
        {
            if (_collidedObject.tag == "SlopeLR")
            {

                _trainRigidbody.transform.rotation = Quaternion.Euler(  _trainRigidbody.transform.eulerAngles.x,
                                                                        tiltAngle,
                                                                        _trainRigidbody.transform.eulerAngles.z
                                                                        );
            } 
            else if (_collidedObject.tag == "SlopeTD")
            {
                _trainRigidbody.transform.rotation = Quaternion.Euler(  tiltAngle,
                                                                        _trainRigidbody.transform.eulerAngles.y,
                                                                        _trainRigidbody.transform.eulerAngles.z
                                                                        );
            } 
            else
            {
                Debug.LogError("Not possible to be on inclide and yet not on a slope");
            }
        }
        else
        {
            _trainRigidbody.transform.rotation = Quaternion.Euler(  0,
                                                                    0,
                                                                    _trainRigidbody.transform.eulerAngles.z
                                                                    );
        }
    }

    private bool CheckInclineChange(TrackType prevTrack, TrackType incomingTrack)
    {
        switch (prevTrack)
        {
            case TrackType.RightUp:
            case TrackType.RightDown:
            case TrackType.LeftUp:
            case TrackType.LeftDown:
            case TrackType.StraightGround:
                if (incomingTrack == TrackType.InclineUp) return true;
                break;
            case TrackType.StraightBridge:
                if (incomingTrack == TrackType.InclineDown) return true;
                break;
            case TrackType.InclineUp:
                if (incomingTrack == TrackType.StraightBridge) return true;
                break;
            case TrackType.InclineDown:
                if (incomingTrack != TrackType.InclineDown && incomingTrack != TrackType.InclineUp && incomingTrack != TrackType.StraightBridge) return true;
                break;
            case TrackType.Nil:
                break;
            default:
                Debug.LogError("Unaccounted Track type for checking");
                break;

        }
        return false;
    }

    //////////////////////////////////////////////////////
    /// STRAIGHT MOVEMENT LOGIC
    //////////////////////////////////////////////////////

    // Called by the CheckInclineAndSetRotation function to make the train move abit more before performing the rotation
    private IEnumerator MoveFixedDistance()
    {
        Vector3 initialPosition = _trainRigidbody.position;
        float distanceTravelled = Vector3.Distance(initialPosition, _trainRigidbody.position);

        while (distanceTravelled < 1.5f)
        {
            yield return null;
            distanceTravelled = Vector3.Distance(initialPosition, _trainRigidbody.position);
        }
    }

    private void MoveTrainStraight(MovementDirection currentDirn)
    {
        switch (currentDirn)
        {
            case MovementDirection.North:
                _trainRigidbody.velocity = new Vector3(0, CurrentSpeed, 0);
                break;
            case MovementDirection.South:
                _trainRigidbody.velocity = new Vector3(0, -CurrentSpeed, 0);
                break;
            case MovementDirection.East:
                _trainRigidbody.velocity = new Vector3(CurrentSpeed, 0, 0);
                break;
            case MovementDirection.West:
                _trainRigidbody.velocity = new Vector3(-CurrentSpeed, 0, 0);
                break;
            default:
                Debug.LogError($"[TrainMovement] {this.name}: Invalid Direction being used to move in a straight line");
                break;
        }
    }

    private void MoveTrainIncline (MovementDirection currDirn, TrackType incline)
    {
        if (incline != TrackType.InclineUp && incline != TrackType.InclineDown)
        {
            Debug.LogError("Non-incline track should not call this MoveTrainIncline!");
        }

        float x = 0;
        float y = 0;
        float z = 0;

        switch (currDirn){
            case MovementDirection.North:
                y = CurrentSpeed;
                break;
            case MovementDirection.South:
                y = -CurrentSpeed;
                break;
            case MovementDirection.East:
                x = CurrentSpeed;
                break;
            case MovementDirection.West:
                x = -CurrentSpeed;
                break;
            default:
                break;
        }

        switch (incline)
        {
            case TrackType.InclineUp:
                z = -CurrentSpeed;
                break;
            case TrackType.InclineDown:
                z = CurrentSpeed;
                break;
            default:
                break;

        }
        _trainRigidbody.velocity = new Vector3(x, y, z);
    }



    //////////////////////////////////////////////////////
    /// CURVE MOVEMENT LOGIC
    //////////////////////////////////////////////////////

    private IEnumerator MoveTrainRightUp(MovementDirection currentDirn)
    {
        if (currentDirn == MovementDirection.East)
        {
            yield return StartCoroutine(MoveAndRotate(true));
            MovementDirn = MovementDirection.North;

        } 
        else if (currentDirn == MovementDirection.South) 
        {
            yield return StartCoroutine(MoveAndRotate(false));
            MovementDirn = MovementDirection.West;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }
        CurveExitCheck();
    }

    private IEnumerator MoveTrainRightDown(MovementDirection currentDirn)
    {
        if (currentDirn == MovementDirection.East)
        {
            yield return StartCoroutine(MoveAndRotate(false));
            MovementDirn = MovementDirection.South;

        }
        else if (currentDirn == MovementDirection.North)
        {
            yield return StartCoroutine(MoveAndRotate(true));
            MovementDirn = MovementDirection.West;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }
        CurveExitCheck();
    }

    private IEnumerator MoveTrainLeftUp(MovementDirection currentDirn)
    {
        if (currentDirn == MovementDirection.West)
        {
            yield return StartCoroutine(MoveAndRotate(false));
            MovementDirn = MovementDirection.North;

        }
        else if (currentDirn == MovementDirection.South)
        {
            yield return StartCoroutine(MoveAndRotate(true));
            MovementDirn = MovementDirection.East;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }
        CurveExitCheck();
    }

    private IEnumerator MoveTrainLeftDown(MovementDirection currentDirn)
    {
        if (currentDirn == MovementDirection.West)
        {
            yield return StartCoroutine(MoveAndRotate(true));
            MovementDirn = MovementDirection.South;

        }
        else if (currentDirn == MovementDirection.North)
        {
            yield return StartCoroutine(MoveAndRotate(false));
            MovementDirn = MovementDirection.East;
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
    /// BACKEND RELATED FUNCTIONS
    //////////////////////////////////////////////////////

    private void UpdateTrainAttribute()
    {
        Vector3 trainPosition = transform.position;
        Quaternion trainRotation = transform.rotation;
        TrainAttribute.SetUnityStats(CurrentSpeed, trainPosition, trainRotation, MovementDirn, MovementState);
    }

    private IEnumerator LoadTrainStartMovement()
    {
        yield return new WaitUntil(() => _trackType != TrackType.Nil);
        StartCoroutine(MoveTrain());
    }

    //////////////////////////////////////////////////////
    /// PUBLIC FUNCTIONS
    //////////////////////////////////////////////////////
    public void DepartTrain(MovementDirection movementDirn)
    {
        MovementDirn = movementDirn;

        _trackType = TrackType.StraightGround;
        MovementState = MovementState.Moving;
        _trainMgr.PlatformExitProcedure();

        StartCoroutine(MoveTrain());
        StopCoroutine(_trainRefuelCoroutine);
    }
}
