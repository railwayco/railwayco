using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class buttonManager : MonoBehaviour
{
    public Button myButton;
    public GameObject train;
    private string stationName;

    // Start is called before the first frame update
    void Start()
    {
        myButton.onClick.AddListener(OnButtonClicked);
    }

    public void getStationName(string stnName)
    {
        stationName = stnName;
    }


    public void OnButtonClicked()
    {
        if (stationName == "Station0")
        {
            train.GetComponent<train_movement>().departTrain("right", stationName);
            Debug.Log("Train will be moving right");
        }
        else
        {
            train.GetComponent<train_movement>().departTrain("left", stationName);
            Debug.Log("Train will be moving left");
    }
        Debug.Log("LESSHGO");
    }
}
