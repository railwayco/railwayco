using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    public new Rigidbody2D rigidbody;
    private float maxSpeed = 5; // Change to enum values once we have more trains
    private float acceleration = 1; // This acceleration Value will vary once we have different trains
    private float accelerationCopy;
    private float currentSpeed = 0;
    private string accelerationDirn;
    private string currentStation; // String for now, to replace with station ID.



    private void Start()
    {
        accelerationCopy = acceleration;

    }

    void Update()
    {
        moveTrain();
    }

    public void departTrain()
    {
        // Will assume the train starts moving to the right.
        // To update Logic on which way to depart once stations' relationship are established.

        currentSpeed += acceleration * Time.deltaTime;

    }

    public void moveTrain()
    {
        if (currentSpeed == 0) return;

        accelerationDirn = "right";

        if (accelerationDirn == "right")
        {
            if (currentSpeed > 0)
            {
                currentSpeed += accelerationCopy * Time.deltaTime;
            }

            if (currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }

            if (currentSpeed < 0)
            {

                currentSpeed = 0;
                Debug.Log("Train has come to a stop");
                accelerationCopy = Mathf.Abs(acceleration);

            }


            rigidbody.velocity = new Vector2(currentSpeed, 0);

        }



        else
        {
            if (currentSpeed < 0)
            {
                currentSpeed -= accelerationCopy * Time.deltaTime;
            }

            if (currentSpeed < -maxSpeed)
            {
                currentSpeed = -maxSpeed;
            }
            if (currentSpeed > 0)
            {

                currentSpeed = 0;
                Debug.Log("Train has come to a stop");
                accelerationCopy = Mathf.Abs(acceleration);

            }
            rigidbody.velocity = new Vector2(currentSpeed, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Station")
        {
            currentStation = other.name;

            if (currentSpeed > 0)
            { 
                accelerationCopy = acceleration * -6;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Station")
        { 
            Debug.Log($"[Train] {this.name} : Exited station {currentStation}.");
            currentStation = null;
        }
    }




}
