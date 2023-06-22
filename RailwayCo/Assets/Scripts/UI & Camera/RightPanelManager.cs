using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Only the Right Panel (under the MainUI) have this script component attached
// All other GameObejcts should find this manager via reference to the RightPanel GameObject.
public class RightPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject cargoTrainStationPanelPrefab;
    [SerializeField] private GameObject cargoTrainOnlyPanelPrefab;
    [SerializeField] private GameObject cargoCellPrefab;

    private LogicManager logicMgr;

    public enum CargoTabOptions
    {
        NIL,
        TRAIN_CARGO,
        STATION_CARGO,
    }

    ////////////////////////////////////////////
    // INITIALISATION
    ////////////////////////////////////////////
    
    private void Awake()
    {
        logicMgr = GameObject.FindGameObjectsWithTag("Logic")[0].GetComponent<LogicManager>();
    }

    ///////////////////////////////////////////////
    // RIGHT PANEL MAINTENANCE
    //////////////////////////////////////////////

    public void resetRightPanel()
    {
        if (!this.gameObject.activeInHierarchy) this.gameObject.SetActive(true);
        DestroyRightPanelChildren();
    }


    private void DestroyRightPanelChildren()
    {
        int noChild = this.transform.childCount;
        for (int i =0; i<noChild; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }

    //////////////////////////////////////////
    // CARGO RELATED RENDERING (OPTIONS)
    ////////////////////////////////////////////


    // Loads the cargo panel, Main entrypoint that determines what gets rendered
    public void loadCargoPanel(GameObject train, GameObject station, CargoTabOptions options)
    {
        resetRightPanel();
        GameObject cargoPanel;

        if (train != null && station == null) // When the train is clicked and  not in the station
        {
            cargoPanel = Instantiate(cargoTrainOnlyPanelPrefab);
            loadTrainOnlyCargoPanel(cargoPanel, train);
        }
        else if (train == null && station != null) // When the selected station has no train
        {
            // TODO: Currently just a placeholder. To be addressed once the yard functionality has been added in
            // TODO: Decide whether to use this panel or come up with another panel that deals with exclusively the Yard stuff
            cargoPanel = Instantiate(cargoTrainStationPanelPrefab);
            loadStationCargoPanelTrainAbsent();
            this.gameObject.SetActive(false);
        }
        else if (train != null && station != null)
        {
            cargoPanel = Instantiate(cargoTrainStationPanelPrefab);
            loadStationCargoPanelTrainPresent(cargoPanel, train, station, options);
        }
        else
        {
            Debug.LogError("This should never happen! At least Either the train or the staion must be valid");
            cargoPanel = null;
        }


        if (cargoPanel)
        {
            cargoPanel.transform.SetParent(this.transform);
            cargoPanel.transform.localPosition = new Vector3(0, 0, 0);
            cargoPanel.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void loadStationCargoPanelTrainAbsent()
    {
        Debug.Log("THERE IS NOTHING TO INSTANTIATE FOR NOW");
        // TODO: When implementing the YARD component, "disable" the buttons and only display the yard Panel
        // Or, we can just give it a new Yard-Only panel while the function below, will have to integrate a new button in :)
    }

    private void loadStationCargoPanelTrainPresent(GameObject cargoPanel, GameObject train, GameObject station, CargoTabOptions options)
    {
        Guid trainGuid = train.GetComponent<TrainManager>().trainGUID;
        Guid stationGuid = station.GetComponent<StationManager>().stationGUID;
        Transform container = getCargoContainer(cargoPanel);
        Cargo[] cargoList = null;
        switch (options)
        {
            case CargoTabOptions.NIL:
            case CargoTabOptions.STATION_CARGO:
                cargoList = logicMgr.getStationCargoList(stationGuid);
                break;
            case CargoTabOptions.TRAIN_CARGO:
                cargoList = logicMgr.getTrainCargoList(trainGuid);
                break;
            default:
                break;

        }
        showCargoDetails(cargoList, container);

        cargoPanel.transform.Find("TrainCargoButton").GetComponent<CargoTabButton>().setTrainAndStationGameObj(train, station);
        cargoPanel.transform.Find("StationCargoButton").GetComponent<CargoTabButton>().setTrainAndStationGameObj(train, station);
        cargoPanel.transform.Find("DepartButton").GetComponent<ButtonTrainDepart>().setTrainToDepart(train);
    }


    private void loadTrainOnlyCargoPanel(GameObject cargoPanel, GameObject train)
    {
        Guid trainGuid = train.GetComponent<TrainManager>().trainGUID;
        Transform container = getCargoContainer(cargoPanel);
        Cargo[] trainCargoList = logicMgr.getTrainCargoList(trainGuid);
        showCargoDetails(trainCargoList, container);
        cargoPanel.transform.Find("TrainMetaInfo").Find("TrainName").GetComponent<Text>().text = train.name;
    }

    //////////////////////////////////////////////////
    // CARGO RELATED BACKEND PROCESSES
    //////////////////////////////////////////////////
    ///
    private Transform getCargoContainer(GameObject cargoPanel)
    {
        // Regardless of the Cargo Panel chosen, the subpanel that contains the container for the cargo should be of this hirarchy
        return cargoPanel.transform.Find("CargoContentPanel").Find("Container");
    }

    /// <summary>
    /// Renders the list of cargo associated with the train and/or station
    /// </summary>
    /// <param name="cargoList"> List of Cargo associated with any GameObject that is supposed to hold such info </param>
    /// <param name="container">
    ///     The container that is part of the chosen CargoPanel. 
    ///     The chosen cargo panel should have the following order inside so that we can get the container:
    ///         (Chosen Cargo Panel)
    ///             `-- CargoContentPanel
    ///                 `-- Container
    /// </param>
    private void showCargoDetails(Cargo[] cargoList, Transform container)
    {
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
    }
}
