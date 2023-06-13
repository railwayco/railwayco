using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class train_movement : MonoBehaviour
{
    public new Rigidbody2D rigidbody;
    private float maxSpeed = 5;
    public float acceleration;
    private float accelerationCopy;
    private float currentSpeed = 0;
    private string accelerationDirn;
    private bool isDecelerating = false;
    private string currentStation;
    private LogicScript ls;


    private void Start()
    {
        accelerationCopy = acceleration;
        ls = GameObject.Find("LogicManager").GetComponent<LogicScript>();
    }

    void Update()
    {
        moveTrain();
    }

    public void departTrain(string accDirn, string stationTriggered) {
        if (stationTriggered != currentStation) return;
        accelerationDirn = accDirn;
        Debug.Log("Departing Train");
        Debug.Log($"Departing Train is heading in the {accDirn} direction");
        if (accDirn == "right")
        {
            currentSpeed += acceleration * Time.deltaTime;
        } else
        {
            currentSpeed -= acceleration * Time.deltaTime;
            
        }
        Debug.Log($"The current speed is {currentSpeed}");
        StartCoroutine(updateTrainStatus());

    }

    public void moveTrain()
    {
        if (currentSpeed == 0) return;
        
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
                ls.updateRewards();

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
                ls.updateRewards();

            }
            rigidbody.velocity = new Vector2(currentSpeed, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.tag == "Station" && currentSpeed != 0)
        {
            
            accelerationCopy = acceleration * -6;

            
        }
        Debug.Log($"colll with {other.name}");
        currentStation = other.name;
    }


    IEnumerator updateTrainStatus()
    {
        yield return new WaitForSeconds(2);
    }


}
