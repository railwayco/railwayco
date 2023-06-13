using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    public Rigidbody2D trainRigidbody;

    // Values are in Absolute terms (No direction)
    private float maxSpeed = 5f; // TODO: Read from Train's Attributes
    private float acceleration = 1; // TODO: Read from Train's attributes
    private float currentSpeed = 0;

    private string currentStation; // String for now, to replace with station ID.
    // Need a boolean to deal with the slowdown for entering the station. (Need size, speed and stuff...)
        // Need to differentiate the state of the train, whether it is slowing, or has fully stopped.
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
        if (trainState != TrainState.STATION_ENTER && currentSpeed > 0)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
    
    }

    /// <summary>
    /// Slows down the train to a stop
    /// </summary>
    private IEnumerator trainStationEnter()
    {
        trainRigidbody.velocity = Vector2.zero;
        int i = 0;
        float decelerationStep = currentSpeed / waypointPath.Length;
        Vector2 currentWaypointPos;
        while (i < waypointPath.Length)
        {
            Debug.LogError($"BP at {waypointPath[i]}");
            Debug.Log(i);
            Debug.Log(currentSpeed);
            Debug.Log(waypointPath[i].position);

            if (movementDirn == Direction.EAST) // Movement Direction. Using waypoints again'
            {
                currentWaypointPos = waypointPath[i].position;
            }
            else if (movementDirn == Direction.WEST)
            {
                currentWaypointPos = waypointPath[waypointPath.Length - i - 1].position;
            }
            else
            {
                Debug.LogError("Train entering the station in the wrong orientation!");
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
    public void departTrain()
    {
        // Will assume the train starts moving to the right.
        // To update Logic on which way to depart once stations' relationship are established.
        movementDirn = Direction.EAST;
        curveType = CurveType.STRAIGHT;
        trainState = TrainState.STATION_DEPART;
        currentSpeed += acceleration * Time.deltaTime;
        StartCoroutine(moveTrain());
    }

    private IEnumerator moveTrain()
    {
        // TODO: Set override: End this MoveTrain Enumerator when train is entering the station.
        while (currentSpeed > 0 && trainState != TrainState.STATION_ENTER)
        {
            switch (curveType)
            {
                // Perform Checks in the respective functions
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
                    Debug.LogError("[Train] MoveTrain Switch Case Not Implemented Error");
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
                Debug.LogError($"[Train] {this.name}: Invalid Direction being used to move in a straight line");
                break;
        }
    }

    private IEnumerator moveTrainRightUp(Direction currentDirn)
    {
 
        if (currentDirn == Direction.EAST)
        {
            yield return StartCoroutine(moveRotate(true));
            movementDirn = Direction.NORTH;

        } 
        else if (currentDirn == Direction.SOUTH) 
        {
            yield return StartCoroutine(moveRotate(false));
            movementDirn = Direction.WEST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }

        // This should be reached only after moveRotate has finished.
        if (curveType != CurveType.STRAIGHT)
        {
            Debug.LogError("moveRotate did not set the curve type from curved to straight");
        }
    }

    private IEnumerator moveTrainRightDown(Direction currentDirn)
    {

        if (currentDirn == Direction.EAST)
        {
            yield return StartCoroutine(moveRotate(false));
            movementDirn = Direction.SOUTH;

        }
        else if (currentDirn == Direction.NORTH)
        {
            yield return StartCoroutine(moveRotate(true));
            movementDirn = Direction.WEST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }

        // This should be reached only after moveRotate has finished.
        if (curveType != CurveType.STRAIGHT)
        {
            Debug.LogError("moveRotate did not set the curve type from curved to straight");
        }
    }

    private IEnumerator moveTrainLeftUp(Direction currentDirn)
    {

        if (currentDirn == Direction.WEST)
        {
            yield return StartCoroutine(moveRotate(false));
            movementDirn = Direction.NORTH;

        }
        else if (currentDirn == Direction.SOUTH)
        {
            yield return StartCoroutine(moveRotate(true));
            movementDirn = Direction.EAST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }

        // This should be reached only after moveRotate has finished.
        if (curveType != CurveType.STRAIGHT)
        {
            Debug.LogError("moveRotate did not set the curve type from curved to straight");
        }
    }

    private IEnumerator moveTrainLeftDown(Direction currentDirn)
    {

        if (currentDirn == Direction.WEST)
        {
            yield return StartCoroutine(moveRotate(true));
            movementDirn = Direction.SOUTH;

        }
        else if (currentDirn == Direction.NORTH)
        {
            yield return StartCoroutine(moveRotate(false));
            movementDirn = Direction.EAST;
        }
        else
        {
            Debug.LogError("Invalid Direction Setting for the train to rotate!");
            yield break;
        }

        // This should be reached only after moveRotate has finished.
        if (curveType != CurveType.STRAIGHT)
        {
            Debug.LogError("moveRotate did not set the curve type from curved to straight");
        }
    }


    private IEnumerator moveRotate(bool rotateLeft)
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
                trainRigidbody.MoveRotation(initialRotationAngle  - degreesRotated);
                currentWaypointPos = waypointPath[waypointPath.Length - i -1].position;
            }
                
            this.transform.position = Vector2.MoveTowards(this.transform.position, currentWaypointPos, currentSpeed * Time.deltaTime );
            float difference = Vector2.Distance((Vector2)this.transform.position, currentWaypointPos);
            if (difference < 0.1f)
            {
                if (degreesRotated == 0 && i !=0)
                {
                    degreesRotated += 5f;
                }
                if (i != 0)
                {
                    degreesRotated += 5;

                }
                i++;
            }
            yield return null;
        }

        // Rotation Finish Condition
        waypointPath = null;
        curveType = CurveType.STRAIGHT;
        degreesRotated = 0;
    }


    // Sets the relevant flags when entering a track or station.
    // Also populate the waypoints if the track is curved so that moveRotate function can utilise it
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Station")
        {
            currentStation = other.name;
            trainState = TrainState.STATION_ENTER;
            int childCount = other.transform.childCount;
            waypointPath = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                waypointPath[i] = other.transform.GetChild(i);
            }
            StartCoroutine(trainStationEnter());
        }

        // Sets the relevant flags so that the MoveTrain function will know how to divert code execution
        // TODO: Switch this to a switch statement... 
        if (other.tag == "Track_Curved_RU")
        {
            curveType = CurveType.RIGHTUP;

            // TOOD: Abstract this part out into a function, to DRY it
            int childCount = other.transform.childCount;
            waypointPath = new Transform[childCount];
            for (int i=0; i< childCount; i++)
            {
                waypointPath[i] = other.transform.GetChild(i);
            }
        }

        if (other.tag == "Track_Curved_RD")
        {
            curveType = CurveType.RIGHTDOWN;
            int childCount = other.transform.childCount;
            waypointPath = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                waypointPath[i] = other.transform.GetChild(i);
            }
        }

        if (other.tag == "Track_Curved_LU")
        {
            curveType = CurveType.LEFTUP;
            int childCount = other.transform.childCount;
            waypointPath = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                waypointPath[i] = other.transform.GetChild(i);
            }
        }
        if (other.tag == "Track_Curved_LD")
        {
            curveType = CurveType.LEFTDOWN;
            int childCount = other.transform.childCount;
            waypointPath = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                waypointPath[i] = other.transform.GetChild(i);
            }
        }

        if (other.tag == "Track_LR" || other.tag == "Track_TD")
        {
            curveType = CurveType.STRAIGHT;
        }

        // Debug Purposes only
        //if (other.tag.Contains("Track"))
        //{
        //    Debug.Log($"[Train] {this.name}: Has entered the track {other.tag}");
        //}
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Station")
        { 
            currentStation = null;
        }
    }
}
