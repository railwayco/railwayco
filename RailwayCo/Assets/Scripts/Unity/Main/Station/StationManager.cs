using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StationManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    private RightPanelManager _rightPanelMgrScript;
    public GameObject _assocTrain { get; private set; }
    public Guid StationGUID { get; private set; }

    private bool _isNew;

    private void Awake()
    {
        GameObject RightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        _rightPanelMgrScript = RightPanel.GetComponent<RightPanelManager>();

        Vector3 position = gameObject.transform.position;
        Station station = _gameManager.GameLogic.GetStationRefByPosition(position);
        Guid stationGuid;
        if (station is null)
        {
            stationGuid = _gameManager.GameLogic.InitStation(this.name, position);
            _isNew = true;
        }
        else
        {
            stationGuid = station.Guid;
            _isNew = false;
        }
        SetStationGUID(stationGuid);
    }

    private void Start()
    {
        if (_isNew) _gameManager.GameLogic.GenerateTracks(this.name);
    }

    // This function should only be set by LogicManager and nowhere else
    public void SetStationGUID(Guid stnGUID)
    {
        StationGUID = stnGUID;
    }


    private void OnMouseUpAsButton()
    {
        _rightPanelMgrScript.LoadCargoPanel(_assocTrain, this.gameObject);
    }

    // Allows the train to set whether it is in the station.
    // Instead of setting when the train is entering/exiting the station,
    // the train is set when fully stopped, and null once it starts moving
    // This is needed so that we know what kind of cargo panel to generate when the station is clicked.
    public void SetTrainInStation(GameObject train)
    {
        _assocTrain = train;
    }
}
