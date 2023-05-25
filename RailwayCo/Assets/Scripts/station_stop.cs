using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class station_stop : MonoBehaviour
{
    public GameObject StationCanvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Show the pop-up containing the information about the station
    private void OnMouseUp()
    {
        Debug.Log("Station Stop is selected");
        StationCanvas.SetActive(true);
        // Maybe show the button to depart beside the cursor

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
