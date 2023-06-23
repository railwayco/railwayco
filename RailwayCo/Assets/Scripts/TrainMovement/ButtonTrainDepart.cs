using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonTrainDepart : MonoBehaviour
{
    public Button button;

    private GameObject trainToDepart;
    private LogicManager logicMgr;

    void Start()
    {
        logicMgr = GameObject.FindGameObjectsWithTag("Logic")[0].GetComponent<LogicManager>();
        button.onClick.AddListener(OnButtonClicked);
    }



    public void OnButtonClicked()
    {
        // Departs the train Object

        Debug.LogWarning("The current train destination is set to Station1 (to the right) for testing.");
        Guid trainGuid = trainToDepart.GetComponent<TrainManager>().trainGUID;
        Guid currStnGuid = trainToDepart.GetComponent<TrainManager>().currentStnGUID;

        logicMgr.setStation1AsDestionation(trainGuid, currStnGuid);

        trainToDepart.GetComponent<TrainMovement>().departTrain();
        GameObject rightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        rightPanel.SetActive(false);
    }

    public void setTrainToDepart(GameObject train)
    {
        trainToDepart = train;
    }
}