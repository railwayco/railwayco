using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class station_stop : MonoBehaviour
{
    public GameObject StationCanvas;


    // Show the pop-up containing the information about the station
    private void OnMouseUp()
    {
        Debug.Log("Station Stop is selected");
        StationCanvas.SetActive(true);
        // Maybe show the button to depart beside the cursor

        string stationName = gameObject.name;
        Debug.Log(stationName);
        StationCanvas.GetComponentInChildren<buttonManager>().getStationName(stationName);
    }


    // Temporary Fix until the UI has been properly implemented.
    // Remove the Canvas once hovered out after 2s
    private void OnMouseExit()
    {
        
        if (StationCanvas.activeInHierarchy)
        {
            StartCoroutine(removeDisplay());
            
        }
    }

    IEnumerator removeDisplay()
    {
        yield return new WaitForSeconds(2);
        StationCanvas.SetActive(false);
    }

}
