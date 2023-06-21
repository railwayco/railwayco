using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StationManager : MonoBehaviour
{ 
    
    private bool trainInStation = false;

    public GameObject rightSubPanelPrefab;
    public GameObject cargoCellPrefab;

    private GameObject RightPanel;
    private LogicManager logicMgr;

    internal Collider2D train;

    private void Start()
    {
        logicMgr = GameObject.FindGameObjectsWithTag("Logic")[0].GetComponent<LogicManager>();
        RightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;

    }


    private void OnMouseUpAsButton()
    {
        RightPanel.GetComponent<RightPanelManager>().resetRightPanel();
        GameObject rightSubPanel = Instantiate(rightSubPanelPrefab);
        rightSubPanel.transform.SetParent(RightPanel.transform);
        rightSubPanel.transform.localPosition = new Vector3(0, 0, 0);
        Transform container = rightSubPanel.transform.Find("Container");

        Cargo[] cargoList = logicMgr.getStationCargoList();
        for (int i = 0; i < cargoList.Length; i++)
        {
            GameObject cargoDetailButton = Instantiate(cargoCellPrefab);
            cargoDetailButton.transform.SetParent(container);

            Guid destStationGUID = cargoList[i].TravelPlan.DestinationStation;
            string dest = logicMgr.getIndividualStationInfo(destStationGUID).Name;
            string cargoType = cargoList[i].Type.ToString();
            string weight = ((int)(cargoList[i].Weight)).ToString();
            string cargoDetail = cargoType + " (" + weight + " t)";

            CurrencyManager currMgr = cargoList[i].CurrencyManager;
            Currency currrency;

            currMgr.CurrencyDict.TryGetValue(CurrencyType.Coin, out currrency);
            string coinAmt = currrency.CurrencyValue.ToString();

            currMgr.CurrencyDict.TryGetValue(CurrencyType.Note, out currrency);
            string noteAmt = currrency.CurrencyValue.ToString();

            currMgr.CurrencyDict.TryGetValue(CurrencyType.NormalCrate, out currrency);
            string nCrateAmt = currrency.CurrencyValue.ToString();

            currMgr.CurrencyDict.TryGetValue(CurrencyType.SpecialCrate, out currrency);
            string sCrateAmt = currrency.CurrencyValue.ToString();

            cargoDetailButton.transform.Find("CargoDetails").GetComponent<Text>().text = cargoDetail;
            cargoDetailButton.transform.Find("Destination").GetComponent<Text>().text = dest;
            cargoDetailButton.transform.Find("CoinAmt").GetComponent<Text>().text = coinAmt;
            cargoDetailButton.transform.Find("NoteAmt").GetComponent<Text>().text = noteAmt;
            cargoDetailButton.transform.Find("NormalCrateAmt").GetComponent<Text>().text = nCrateAmt;
            cargoDetailButton.transform.Find("SpecialCrateAmt").GetComponent<Text>().text = sCrateAmt;
        }

        rightSubPanel.transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[Station] {this.name}: Train {collision.name} is in the station");
        train = collision;
        trainInStation = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log($"[Station] {this.name}: Train {collision.name} has left the station");
        trainInStation = false;
        train = null;
    }


}
