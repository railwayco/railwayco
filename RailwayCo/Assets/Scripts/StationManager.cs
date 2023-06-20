using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StationManager : MonoBehaviour
{ 
    
    private bool trainInStation = false;
    internal Collider2D train;
    

    private void OnMouseUpAsButton()
    {
        Debug.Log("Station clicked");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[Station] {this.name}: Train {collision.name} is in the station");
        train = collision;
        trainInStation = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log($"[Station] {this.name}: Train {collision.name} has left the station");
        trainInStation = false;
        train = null;
    }


}
