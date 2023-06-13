using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    public Rigidbody2D trainRigidbody;

    // Values are in Absolute terms (direction independent)
    private float maxSpeed = 5f; // TODO: Read from Train's Attributes
    private float acceleration = 1; // TODO: Read from Train's attributes
    private float currentSpeed = 0;

    private string currentStation; // String for now, to replace with station ID.
    // TODO in future (With Station): Deploy and move off in the right direction. (Right now its pre-determined to move only to the right)

    private Direction movementDirn;
    private CurveType curveType;
    private TrainState trainState;

    private Transform[] waypointPath;

    enum Direction
    {
        NORTH,
        SOUTH,
        EAST,
        WEST,
    }

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

    void Update()
    {
        if (trainState == TrainState.STATION_DEPART)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
    
    }

    /// <summary>
    /// Slows down the train to a stop. Triggered upon entering the station
    /// </summary>
    private IEnumerator trainStationEnter()
    {
        trainRigidbody.velocity = Vector2.zero; // Removes residual motion from staight-line movement.
        int i = 0;
        float decelerationStep = currentSpeed / waypointPath.Length;
        Vector2 currentWaypointPos;
        while (i < waypointPath.Length && currentSpeed > 0)
        {

            if (movementDirn == Direction.EAST)
            {
                currentWaypointPos = waypointPath[i].position;
            }
            else if (movementDirn == Direction.WEST)
            {
                currentWaypointPos = waypointPath[waypointPath.Length - i - 1].position;
            }
            else
            {
                Debug.LogError($"[TrainMovement] {this.name}: Train entering the station in the wrong orientation!");
                yield break;
            }

            this.transform.position = Vector2.MoveTowards(this.transform.position, currentWaypointPos, currentSpeed * Time.deltaTime);
            float difference = Vector2.Distance((Vector2)this.transform.position, currentWaypointPos);
            if (difference < 0.1f)
            {
                currentSpeed -= decelerationStep;
                i++;
            }
            yield return null;
        }

        if (currentSpeed < 0) currentSpeed = 0;
        waypointPath = null;
        trainState = TrainState.STATION_STOPPED;

    }

    /// <summary>
    /// Called by the Depart routine (external)
    /// </summary>
    public void departTrain()
    {
        // Will assume the train starts moving to the right.
        // To update Logic on depart checklist and direcion once stations' relationship are established.
        movementDirn = Direction.EAST;
        curveType = CurveType.STRAIGHT;
        trainState = TrainState.STATION_DEPART;
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
                    moveTrainStraight(movementDirn);
                    break;
                case CurveType.RIGHTUP:
                    yield return StartCoroutine(moveTrainRightUp(movementDirn));
                    break;
                case CurveType.RIGHTDOWN:
                    yield return StartCoroutine(moveTrainRightDown(movementDirn));
                    break;
                case CurveType.LEFTUP:
                    yield return StartCoroutine(moveTrainLeftUp(movementDirn));
                    break;
                case CurveType.LEFTDOWN:
                    yield return StartCoroutine(moveTrainLeftDown(movementDirn));
                    break;
                default:
                    Debug.LogError("[TrainMovement] MoveTrain Switch Case Not Implemented Error");
                    yield break;
            }
            yield return null;
        }
    }


    private void moveTrainStraight(Direction currentDirn)
    {
        switch (currentDirn)
        {
            case Direction.NORTH:
                trainRigidbody.velocity = new Vector2(0, currentSpeed);
                break;
            case Direction.SOUTH:
                trainRigidbody.velocity = new Vector2(0, -currentSpeed);
                break;
            case Direction.EAST:
                trainRigidbody.velocity = new Vector2(currentSpeed, 0);
                break;
            case Direction.WEST:
                trainRigidbody.velocity = new Vector2(-currentSpeed, 0);
                break;
            default:
                Debug.LogError($"[TrainMovement] {this.name}: Invalid Direction being used to move in a straight line");
                break;
        }
    }

    private IEnumerator moveTrainRightUp(Direction currentDirn)
    {
        if (currentDirn == Direction.EAST)
        {
            yield return StartCoroutine(moveAndRotate(true));
            movementDirn = Direction.NORTH;

        } 
        else if (currentDirn == Direction.SOUTH) 
        {
            yield return StartCoroutine(moveAndRotate(false));
            movementDirn = Direction.WEST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }
        curveExitCheck();
    }

    private IEnumerator moveTrainRightDown(Direction currentDirn)
    {
        if (currentDirn == Direction.EAST)
        {
            yield return StartCoroutine(moveAndRotate(false));
            movementDirn = Direction.SOUTH;

        }
        else if (currentDirn == Direction.NORTH)
        {
            yield return StartCoroutine(moveAndRotate(true));
            movementDirn = Direction.WEST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }
        curveExitCheck();
    }

    private IEnumerator moveTrainLeftUp(Direction currentDirn)
    {
        if (currentDirn == Direction.WEST)
        {
            yield return StartCoroutine(moveAndRotate(false));
            movementDirn = Direction.NORTH;

        }
        else if (currentDirn == Direction.SOUTH)
        {
            yield return StartCoroutine(moveAndRotate(true));
            movementDirn = Direction.EAST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }
        curveExitCheck();
    }

    private IEnumerator moveTrainLeftDown(Direction currentDirn)
    {
        if (currentDirn == Direction.WEST)
        {
            yield return StartCoroutine(moveAndRotate(true));
            movementDirn = Direction.SOUTH;

        }
        else if (currentDirn == Direction.NORTH)
        {
            yield return StartCoroutine(moveAndRotate(false));
            movementDirn = Direction.EAST;
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
                
            this.transform.position = Vector2.MoveTowards(this.transform.position, currentWaypointPos, currentSpeed * Time.deltaTime );
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
                currentStation = other.name;
                trainState = TrainState.STATION_ENTER;
                StartCoroutine(trainStationEnter());
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


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Station")
        { 
            currentStation = null;
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
