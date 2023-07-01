using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _trainRigidbody;
    // TODO in future (With Station): Deploy and move off in the right direction. (Right now its pre-determined to move only to the right)

    // Values are in Absolute terms (direction independent)
    private float _acceleration = 1; // TODO: Read from Train's attributes
    public float CurrentSpeed { get;  private set; }

    public TrainDirection MovementDirn { get; private set; }
    private CurveType _curveType;
    private TrainState _trainState;

    public float MaxSpeed { get; private set; }

    private Transform[] _waypointPath;

    // The 4 kinds of curved tracks and the straights
    private enum CurveType
    {
        RIGHTUP,
        RIGHTDOWN,
        LEFTUP,
        LEFTDOWN,
        STRAIGHT
    }

    private enum TrainState
    {
        STATION_ENTER,
        STATION_STOPPED,
        STATION_DEPART
    }

    /////////////////////////////////////////////////////////
    // FUNCTIONS
    /////////////////////////////////////////////////////////

    private void Start()
    {
        MaxSpeed = 5;
    }

    void Update()
    {
        if (_trainState == TrainState.STATION_DEPART)
        {
            CurrentSpeed += _acceleration * Time.deltaTime;
        }
        if (CurrentSpeed > MaxSpeed)
        {
            CurrentSpeed = MaxSpeed;
        }
    }

    /// <summary>
    /// Slows down the train to a stop. Triggered upon entering the station
    /// </summary>
    private IEnumerator TrainStationEnter(GameObject station)
    {
        _trainRigidbody.velocity = Vector2.zero; // Removes residual motion from staight-line movement.
        int i = 0;
        float decelerationStep = CurrentSpeed / _waypointPath.Length;
        Vector2 currentWaypointPos;
        while (i < _waypointPath.Length && CurrentSpeed > 0)
        {

            if (MovementDirn == TrainDirection.EAST)
            {
                currentWaypointPos = _waypointPath[i].position;
            }
            else if (MovementDirn == TrainDirection.WEST)
            {
                currentWaypointPos = _waypointPath[_waypointPath.Length - i - 1].position;
            }
            else
            {
                Debug.LogError($"[TrainMovement] {this.name}: Train entering the station in the wrong orientation!");
                yield break;
            }

            this.transform.position = Vector2.MoveTowards(this.transform.position, currentWaypointPos, CurrentSpeed * Time.deltaTime);
            float difference = Vector2.Distance((Vector2)this.transform.position, currentWaypointPos);
            if (difference < 0.1f)
            {
                CurrentSpeed -= decelerationStep;
                i++;
            }
            yield return null;
        }

        if (CurrentSpeed < 0) CurrentSpeed = 0;
        _waypointPath = null;
        _trainState = TrainState.STATION_STOPPED;

        this.GetComponent<TrainManager>().StationEnterProcedure(station);
    }

    /// <summary>
    /// Called by the Depart routine (external)
    /// </summary>
    public void DepartTrain(bool isRight)
    {
        if (isRight) MovementDirn = TrainDirection.EAST;
        else MovementDirn = TrainDirection.WEST;

        _curveType = CurveType.STRAIGHT;
        _trainState = TrainState.STATION_DEPART;


        this.GetComponent<TrainManager>().StationExitProcedure(null);
        StartCoroutine(MoveTrain());
    }

    /// <summary>
    /// Called by departTrain to depart from the station
    /// </summary>
    private IEnumerator MoveTrain()
    {
        while (_trainState == TrainState.STATION_DEPART)
        {
            switch (_curveType)
            {
                case CurveType.STRAIGHT:
                    MoveTrainStraight(MovementDirn);
                    break;
                case CurveType.RIGHTUP:
                    yield return StartCoroutine(MoveTrainRightUp(MovementDirn));
                    break;
                case CurveType.RIGHTDOWN:
                    yield return StartCoroutine(MoveTrainRightDown(MovementDirn));
                    break;
                case CurveType.LEFTUP:
                    yield return StartCoroutine(MoveTrainLeftUp(MovementDirn));
                    break;
                case CurveType.LEFTDOWN:
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
                _trainRigidbody.velocity = new Vector2(0, CurrentSpeed);
                break;
            case TrainDirection.SOUTH:
                _trainRigidbody.velocity = new Vector2(0, -CurrentSpeed);
                break;
            case TrainDirection.EAST:
                _trainRigidbody.velocity = new Vector2(CurrentSpeed, 0);
                break;
            case TrainDirection.WEST:
                _trainRigidbody.velocity = new Vector2(-CurrentSpeed, 0);
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
        _trainRigidbody.velocity = Vector2.zero; // Removes the residual velocity that arises from moving straight, or it will cause a curved path between waypoints
        Vector2 currentWaypointPos;

        while (i < _waypointPath.Length)
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
                currentWaypointPos = _waypointPath[_waypointPath.Length - i -1].position;
            }
                
            this.transform.position = Vector2.MoveTowards(this.transform.position, currentWaypointPos, CurrentSpeed * Time.deltaTime );
            float difference = Vector2.Distance((Vector2)this.transform.position, currentWaypointPos);
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
        _curveType = CurveType.STRAIGHT;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        // Populate waypoints
            // 1. Curved tracks for moveRotate()
            // 2. Stations for the slowdown effect in trainStationEnter()
            // Else the waypoints will be an empty one
        int childCount = other.transform.childCount;
        _waypointPath = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            _waypointPath[i] = other.transform.GetChild(i);
        }

        // Sets the relevant flags so that the MoveTrain function will know how to divert code execution
        switch (other.tag)
        {
            case "Station":
                _trainState = TrainState.STATION_ENTER;
                StartCoroutine(TrainStationEnter(other.gameObject));
                break;
            case "Track_Curved_RU":
                _curveType = CurveType.RIGHTUP;
                break;
            case "Track_Curved_RD":
                _curveType = CurveType.RIGHTDOWN;
                break;
            case "Track_Curved_LU":
                _curveType = CurveType.LEFTUP;
                break;
            case "Track_Curved_LD":
                _curveType = CurveType.LEFTDOWN;
                break;

            case "Track_LR":
            case "Track_TD":
                _curveType = CurveType.STRAIGHT;
                break;
            default:
                Debug.LogError($"[TrainMovement] {this.name}: Invalid Tag in the Train's Trigger Zone");
                break;
        }
    }


    // Called after moveRotate has finished.
    private void curveExitCheck()
    {
        if (_curveType != CurveType.STRAIGHT)
        {
            Debug.LogError("moveRotate did not set the curve type from curved to straight");
        }
    }


}
