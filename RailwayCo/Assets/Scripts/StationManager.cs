using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StationManager : MonoBehaviour
{ 
    private RightPanelManager rightPanelMgrScript;
    internal GameObject assocTrain;

    private void Start()
    {
        GameObject RightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        rightPanelMgrScript = RightPanel.GetComponent<RightPanelManager>();
    }


    private void OnMouseUpAsButton()
    {
        rightPanelMgrScript.loadCargoPanel(assocTrain, this.gameObject);
    }

    // Allows the train to set whether it is in the station.
    // Instead of setting when the train is entering/exiting the station,
    // the train is set when fully stopped, and null once it starts moving
    // This is needed so that we know what kind of cargo panel to generate when the statoin is clicked.
    public void setTrainInStation(GameObject train)
    {
        assocTrain = train;
    }
}
