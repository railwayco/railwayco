using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class train_movement : MonoBehaviour
{
    public Rigidbody2D rigidbody;
    private bool reaching_station = false;
    private bool justDeparted = false;
    private float maxSpeed = 5;
    public float acceleration;
    private float currentSpeed = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveTrain();
    }

    public void departTrain() {
        Debug.Log("Departing Train");
        currentSpeed += acceleration * Time.deltaTime;
        StartCoroutine(updateTrainStatus());

    }

    public void moveTrain()
    {
        if (currentSpeed > 0)
        {
            currentSpeed += acceleration*Time.deltaTime;
        }

        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
        rigidbody.velocity = new Vector2(currentSpeed, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Station" && currentSpeed > 0)
        {
            acceleration = acceleration * -5;
        }
        Debug.Log("colll");
    }


    IEnumerator updateTrainStatus()
    {
        yield return new WaitForSeconds(2);
        justDeparted = false;
    }

    
}
