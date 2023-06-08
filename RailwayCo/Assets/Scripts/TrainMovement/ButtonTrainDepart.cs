using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonTrainDepart : MonoBehaviour
{
    public Button button;
    private Collider2D trainToDepart;
    

    void Start()
    {
        button.onClick.AddListener(OnButtonClicked);
        trainToDepart= GetComponentInParent<StationTrainDepart>().train;
    }



    public void OnButtonClicked()
    {
        // Departs the train Object
        Debug.Log("Train will be moving right for now to test the tracks functionality.");
        trainToDepart.GetComponent<TrainMovement>().departTrain();
    }
}