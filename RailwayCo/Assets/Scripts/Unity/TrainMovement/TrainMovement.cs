using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    private LogicManager logicMgr;
    [SerializeField] private Rigidbody2D trainRigidbody;
    // TODO in future (With Station): Deploy and move off in the right direction. (Right now its pre-determined to move only to the right)

    // Values are in Absolute terms (direction independent)
    private float maxSpeed = 5f; // TODO: Read from Train's Attributes
    private float acceleration = 1; // TODO: Read from Train's attributes
    public float CurrentSpeed { get;  private set; }

    public TrainDirection MovementDirn { get; private set; }
    private CurveType curveType;
    private TrainState trainState;

    public GameObject CurrentStation { get; private set; }
    public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }

    private Transform[] waypointPath;

    // The 4 kinds of curved tracks and the straights
    enum CurveType
    {
        RIGHTUP,
        RIGHTDOWN,
        LEFTUP,
        LEFTDOWN,
        STRAIGHT
    }

    enum TrainState
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
        logicMgr = GameObject.FindGameObjectsWithTag("Logic")[0].GetComponent<LogicManager>();
    }

    void Update()
    {
        if (trainState == TrainState.STATION_DEPART)
        {
            CurrentSpeed += acceleration * Time.deltaTime;
        }
        if (CurrentSpeed > MaxSpeed)
        {
            CurrentSpeed = MaxSpeed;
        }
    }

    /// <summary>
    /// Slows down the train to a stop. Triggered upon entering the station
    /// </summary>
    private IEnumerator trainStationEnter(GameObject station)
    {
        trainRigidbody.velocity = Vector2.zero; // Removes residual motion from staight-line movement.
        int i = 0;
        float decelerationStep = CurrentSpeed / waypointPath.Length;
        Vector2 currentWaypointPos;
        while (i < waypointPath.Length && CurrentSpeed > 0)
        {

            if (MovementDirn == TrainDirection.EAST)
            {
                currentWaypointPos = waypointPath[i].position;
            }
            else if (MovementDirn == TrainDirection.WEST)
            {
                currentWaypointPos = waypointPath[waypointPath.Length - i - 1].position;
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
        waypointPath = null;
        trainState = TrainState.STATION_STOPPED;
        CurrentStation = station.gameObject;
        StationManager stnMgr = CurrentStation.GetComponent<StationManager>();
        stnMgr.setTrainInStation(this.gameObject);
        this.GetComponent<TrainManager>().setCurrentStationGUID(stnMgr.stationGUID);
        logicMgr.processCargo(this.GetComponent<TrainManager>().trainGUID);
    }

    /// <summary>
    /// Called by the Depart routine (external)
    /// </summary>
    public void departTrain(bool isRight)
    {
        if (isRight) MovementDirn = TrainDirection.EAST;
        else MovementDirn = TrainDirection.WEST;

        curveType = CurveType.STRAIGHT;
        trainState = TrainState.STATION_DEPART;
        CurrentStation.GetComponent<StationManager>().setTrainInStation(null);
        this.GetComponent<TrainManager>().setCurrentStationGUID(Guid.Empty);
        CurrentStation = null;
        StartCoroutine(moveTrain());
    }

    /// <summary>
    /// Called by departTrain to depart from the station
    /// </summary>
    private IEnumerator moveTrain()
    {
        while (trainState == TrainState.STATION_DEPART)
        {
            switch (curveType)
            {
                case CurveType.STRAIGHT:
                    moveTrainStraight(MovementDirn);
                    break;
                case CurveType.RIGHTUP:
                    yield return StartCoroutine(moveTrainRightUp(MovementDirn));
                    break;
                case CurveType.RIGHTDOWN:
                    yield return StartCoroutine(moveTrainRightDown(MovementDirn));
                    break;
                case CurveType.LEFTUP:
                    yield return StartCoroutine(moveTrainLeftUp(MovementDirn));
                    break;
                case CurveType.LEFTDOWN:
                    yield return StartCoroutine(moveTrainLeftDown(MovementDirn));
                    break;
                default:
                    Debug.LogError("[TrainMovement] MoveTrain Switch Case Not Implemented Error");
                    yield break;
            }
            yield return null;
        }
    }


    private void moveTrainStraight(TrainDirection currentDirn)
    {
        switch (currentDirn)
        {
            case TrainDirection.NORTH:
                trainRigidbody.velocity = new Vector2(0, CurrentSpeed);
                break;
            case TrainDirection.SOUTH:
                trainRigidbody.velocity = new Vector2(0, -CurrentSpeed);
                break;
            case TrainDirection.EAST:
                trainRigidbody.velocity = new Vector2(CurrentSpeed, 0);
                break;
            case TrainDirection.WEST:
                trainRigidbody.velocity = new Vector2(-CurrentSpeed, 0);
                break;
            default:
                Debug.LogError($"[TrainMovement] {this.name}: Invalid Direction being used to move in a straight line");
                break;
        }
    }

    private IEnumerator moveTrainRightUp(TrainDirection currentDirn)
    {
        if (currentDirn == TrainDirection.EAST)
        {
            yield return StartCoroutine(moveAndRotate(true));
            MovementDirn = TrainDirection.NORTH;

        } 
        else if (currentDirn == TrainDirection.SOUTH) 
        {
            yield return StartCoroutine(moveAndRotate(false));
            MovementDirn = TrainDirection.WEST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }
        curveExitCheck();
    }

    private IEnumerator moveTrainRightDown(TrainDirection currentDirn)
    {
        if (currentDirn == TrainDirection.EAST)
        {
            yield return StartCoroutine(moveAndRotate(false));
            MovementDirn = TrainDirection.SOUTH;

        }
        else if (currentDirn == TrainDirection.NORTH)
        {
            yield return StartCoroutine(moveAndRotate(true));
            MovementDirn = TrainDirection.WEST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }
        curveExitCheck();
    }

    private IEnumerator moveTrainLeftUp(TrainDirection currentDirn)
    {
        if (currentDirn == TrainDirection.WEST)
        {
            yield return StartCoroutine(moveAndRotate(false));
            MovementDirn = TrainDirection.NORTH;

        }
        else if (currentDirn == TrainDirection.SOUTH)
        {
            yield return StartCoroutine(moveAndRotate(true));
            MovementDirn = TrainDirection.EAST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }
        curveExitCheck();
    }

    private IEnumerator moveTrainLeftDown(TrainDirection currentDirn)
    {
        if (currentDirn == TrainDirection.WEST)
        {
            yield return StartCoroutine(moveAndRotate(true));
            MovementDirn = TrainDirection.SOUTH;

        }
        else if (currentDirn == TrainDirection.NORTH)
        {
            yield return StartCoroutine(moveAndRotate(false));
            MovementDirn = TrainDirection.EAST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }
        curveExitCheck();
    }


    private IEnumerator moveAndRotate(bool rotateLeft)
    {        
        int i = 0;
        float degreesRotated = 0;
        float initialRotationAngle = trainRigidbody.rotation;
        trainRigidbody.velocity = Vector2.zero; // Removes the residual velocity that arises from moving straight, or it will cause a curved path between waypoints
        Vector2 currentWaypointPos;

        while (i < waypointPath.Length)
        {
            if (degreesRotated > 90) degreesRotated = 90;

            if (rotateLeft)
            {
                trainRigidbody.MoveRotation(initialRotationAngle + degreesRotated);
                currentWaypointPos = waypointPath[i].position;
            }
            else
            {
                trainRigidbody.MoveRotation(initialRotationAngle - degreesRotated);
                currentWaypointPos = waypointPath[waypointPath.Length - i -1].position;
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
        waypointPath = null;
        curveType = CurveType.STRAIGHT;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        // Populate waypoints
            // 1. Curved tracks for moveRotate()
            // 2. Stations for the slowdown effect in trainStationEnter()
            // Else the waypoints will be an empty one
        int childCount = other.transform.childCount;
        waypointPath = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            waypointPath[i] = other.transform.GetChild(i);
        }

        // Sets the relevant flags so that the MoveTrain function will know how to divert code execution
        switch (other.tag)
        {
            case "Station":
                trainState = TrainState.STATION_ENTER;
                StartCoroutine(trainStationEnter(other.gameObject));
                break;
            case "Track_Curved_RU":
                curveType = CurveType.RIGHTUP;
                break;
            case "Track_Curved_RD":
                curveType = CurveType.RIGHTDOWN;
                break;
            case "Track_Curved_LU":
                curveType = CurveType.LEFTUP;
                break;
            case "Track_Curved_LD":
                curveType = CurveType.LEFTDOWN;
                break;

            case "Track_LR":
            case "Track_TD":
                curveType = CurveType.STRAIGHT;
                break;
            default:
                Debug.LogError($"[TrainMovement] {this.name}: Invalid Tag in the Train's Trigger Zone");
                break;
        }
    }


    // Called after moveRotate has finished.
    private void curveExitCheck()
    {
        if (curveType != CurveType.STRAIGHT)
        {
            Debug.LogError("moveRotate did not set the curve type from curved to straight");
        }
    }


}
