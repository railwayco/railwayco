using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TrainDepartButton : MonoBehaviour
{
    [SerializeField] private Button _button;

    private GameObject _trainToDepart;
    private LogicManager _logicMgr;
    private Guid _currStnGuid;
    private Guid _destStnGuid = Guid.Empty;
    private bool _isRight;

    public void SetTrainDepartInformation(GameObject train)
    {
        _trainToDepart = train;
    }
    void Awake()
    {
        _logicMgr = GameObject.FindGameObjectsWithTag("Logic")[0].GetComponent<LogicManager>();
        _button.onClick.AddListener(OnButtonClicked);
    }

    void Start()
    {
        _currStnGuid = _trainToDepart.GetComponent<TrainManager>().CurrentStnGUID;
        Station stationObject = _logicMgr.GetIndividualStationInfo(_currStnGuid);
        HashSet<Guid> neighbourGuids = stationObject.StationHelper.GetAll();
        foreach(Guid neighbour in neighbourGuids)
        {
            StationOrientation neighbourOrientation = stationObject.StationHelper.GetObject(neighbour);
            string neighbourName = _logicMgr.GetIndividualStationInfo(neighbour).Name;


            if ((neighbourOrientation == StationOrientation.Tail_Tail 
                || neighbourOrientation == StationOrientation.Tail_Head)
                && _button.name == "LeftDepartButton")
            {
                _destStnGuid = neighbour;
                _isRight = false;
                _button.GetComponentInChildren<Text>().text = "Depart to " + neighbourName;
                break;
            }
            else if ((neighbourOrientation == StationOrientation.Head_Head 
                || neighbourOrientation == StationOrientation.Head_Tail) 
                && _button.name == "RightDepartButton")
            {
                _destStnGuid = neighbour;
                _isRight = true;
                _button.GetComponentInChildren<Text>().text = "Depart to " + neighbourName;
                break;
            }
        }
    }

    public void OnButtonClicked()
    {
        // Departs the train Object

        if (_destStnGuid == Guid.Empty) return;

        Guid trainGuid = _trainToDepart.GetComponent<TrainManager>().TrainGUID;
        _logicMgr.SetStationAsDestination(trainGuid, _currStnGuid, _destStnGuid);

        _trainToDepart.GetComponent<TrainMovement>().DepartTrain(_isRight);
        GameObject rightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        rightPanel.SetActive(false);
    }

   
}