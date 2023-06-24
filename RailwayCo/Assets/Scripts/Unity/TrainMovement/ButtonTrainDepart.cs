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
    private Guid currStnGuid;
    private Guid destStnGuid = Guid.Empty;
    private bool isRight;

    void Awake()
    {
        logicMgr = GameObject.FindGameObjectsWithTag("Logic")[0].GetComponent<LogicManager>();
        button.onClick.AddListener(OnButtonClicked);
    }

    void Start()
    {
        currStnGuid = trainToDepart.GetComponent<TrainManager>().currentStnGUID;
        Station stationObject = logicMgr.getIndividualStationInfo(currStnGuid);
        HashSet<Guid> neighbourGuids = stationObject.StationHelper.GetAllGuids();
        foreach(Guid neighbour in neighbourGuids)
        {
            StationOrientation neighbourOrientation = stationObject.StationHelper.GetObject(neighbour);
            string neighbourName = logicMgr.getIndividualStationInfo(neighbour).Name;


            if ((neighbourOrientation == StationOrientation.Tail_Tail 
                || neighbourOrientation == StationOrientation.Tail_Head)
                && button.name == "LeftDepartButton")
            {
                destStnGuid = neighbour;
                isRight = false;
                button.GetComponentInChildren<Text>().text = "Depart to " + neighbourName;
                break;
            }
            else if ((neighbourOrientation == StationOrientation.Head_Head 
                || neighbourOrientation == StationOrientation.Head_Tail) 
                && button.name == "RightDepartButton")
            {
                destStnGuid = neighbour;
                isRight = true;
                button.GetComponentInChildren<Text>().text = "Depart to " + neighbourName;
                break;
            }
        }
    }

    public void OnButtonClicked()
    {
        // Departs the train Object

        if (destStnGuid == Guid.Empty) return;

        Guid trainGuid = trainToDepart.GetComponent<TrainManager>().trainGUID;
        logicMgr.setStationAsDestination(trainGuid, currStnGuid, destStnGuid);

        trainToDepart.GetComponent<TrainMovement>().departTrain(isRight);
        GameObject rightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        rightPanel.SetActive(false);
    }

    public void setTrainToDepart(GameObject train)
    {
        trainToDepart = train;
    }
}