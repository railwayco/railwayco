using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StationTrainDepart : MonoBehaviour
{ 
    // Prefab of the canvas continng the "Depart" button
    public GameObject DepartButtonCanvasPrefab;
    // Instance of the canvas prefab
    private GameObject buttonCanvas;
    private bool trainInStation = false;
    internal Collider2D train;
    

    // Show the side panel containing the information about the station
    private void OnMouseUpAsButton()
    {
        
    }


    //Remove the Canvas containing the button once hovered out of the station after 2s
    private void OnMouseExit()
    {
        if (buttonCanvas)
        {
            StartCoroutine(removeButtonDisplay());
        }
    }

    IEnumerator removeButtonDisplay()
    {
        yield return new WaitForSeconds(2);
        Destroy(buttonCanvas);
        buttonCanvas = null;
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
