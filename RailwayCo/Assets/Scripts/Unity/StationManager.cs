using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StationManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private RightPanelManager rightPanelMgrScript;
    internal GameObject assocTrain;
    public Guid stationGUID { get; private set; }

    private bool isNew;

    private void Awake()
    {
        GameObject RightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        rightPanelMgrScript = RightPanel.GetComponent<RightPanelManager>();

        Vector3 position = gameObject.transform.position;
        Station station = gameManager.GameLogic.GetStationRefByPosition(position);
        Guid stationGuid;
        if (station is null)
        {
            stationGuid = gameManager.GameLogic.InitStation(this.name, position);
            isNew = true;
        }
        else
        {
            stationGuid = station.Guid;
            isNew = false;
        }
        setStationGUID(stationGuid);
    }

    private void Start()
    {
        if (isNew) gameManager.GameLogic.GenerateTracks(this.name);
    }

    // This function should only be set by LogicManager and nowhere else
    public void setStationGUID(Guid stnGUID)
    {
        stationGUID = stnGUID;
    }


    private void OnMouseUpAsButton()
    {
        rightPanelMgrScript.loadCargoPanel(assocTrain, this.gameObject);
    }

    // Allows the train to set whether it is in the station.
    // Instead of setting when the train is entering/exiting the station,
    // the train is set when fully stopped, and null once it starts moving
    // This is needed so that we know what kind of cargo panel to generate when the station is clicked.
    public void setTrainInStation(GameObject train)
    {
        assocTrain = train;
    }
}
