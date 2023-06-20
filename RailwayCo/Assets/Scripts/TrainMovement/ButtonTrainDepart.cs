using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonTrainDepart : MonoBehaviour
{
    public Button button;

    private GameObject trainToDepart;

    void Start()
    {
        button.onClick.AddListener(OnButtonClicked);
    }



    public void OnButtonClicked()
    {
        // Departs the train Object
        Debug.Log("Train will be moving right for now to test the tracks functionality.");
        trainToDepart.GetComponent<TrainMovement>().departTrain();
    }

    public void setTrainToDepart(GameObject train)
    {
        trainToDepart = train;
    }
}