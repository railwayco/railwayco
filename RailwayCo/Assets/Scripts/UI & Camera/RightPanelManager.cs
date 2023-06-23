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
    [SerializeField] private GameObject cargoStationOnlyPanelPrefab;
    [SerializeField] private GameObject cargoTrainOnlyPanelPrefab;
    [SerializeField] private GameObject cargoCellPrefab;

    private LogicManager logicMgr;

    // Only meaningful in the context of the CargoTrainStationPanel, and only when the train is stopped at the station
    // to show the correct association (either from the station, train or the yard)
    // Otherwise, it has no effect
    // Used only by the CargoTab Options
    private CargoTabOptions cargoTabOptions = CargoTabOptions.NIL;
    public enum CargoTabOptions
    {
        NIL,
        TRAIN_CARGO,
        STATION_CARGO,
        YARD_CARGO
    }

    // Set by CargoTabButton.cs. Used only in the case when train is in the station
    public void setChosenCargoTab(CargoTabOptions cargooptions)
    {
        cargoTabOptions = cargooptions;
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
    public void loadCargoPanel(GameObject train, GameObject station)
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
            cargoPanel = Instantiate(cargoStationOnlyPanelPrefab);
            loadStationCargoPanelTrainAbsent(cargoPanel,station);
        }
        else if (train != null && station != null)
        {
            cargoPanel = Instantiate(cargoTrainStationPanelPrefab);
            loadStationCargoPanelTrainPresent(cargoPanel, train, station);
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

    private void loadStationCargoPanelTrainAbsent(GameObject cargoPanel, GameObject station)
    {
        Guid stationGuid = station.GetComponent<StationManager>().stationGUID;
        Transform container = getCargoContainer(cargoPanel);
        List<Cargo> stationFullCargoList = logicMgr.getStationCargoList(stationGuid);
        List<Cargo> cargoList = getStationSubCargo(stationFullCargoList, false);
        showCargoDetails(cargoList, container, true, Guid.Empty, stationGuid);
    }

    private void loadStationCargoPanelTrainPresent(GameObject cargoPanel, GameObject train, GameObject station)
    {
        Guid trainGuid = train.GetComponent<TrainManager>().trainGUID;
        Guid stationGuid = station.GetComponent<StationManager>().stationGUID;
        Transform container = getCargoContainer(cargoPanel);
        List<Cargo> fullStationCargoList = new List<Cargo>();
        List<Cargo> cargoList = new List<Cargo>();
        switch (cargoTabOptions)
        {
            case CargoTabOptions.NIL:
            case CargoTabOptions.STATION_CARGO:
                fullStationCargoList = logicMgr.getStationCargoList(stationGuid);
                cargoList = getStationSubCargo(fullStationCargoList, true);
                break;
            case CargoTabOptions.YARD_CARGO:
                fullStationCargoList = logicMgr.getStationCargoList(stationGuid);
                cargoList = getStationSubCargo(fullStationCargoList, false);
                break;
            case CargoTabOptions.TRAIN_CARGO:
                cargoList = logicMgr.getTrainCargoList(trainGuid);
                break;
            default:
                break;

        }
        showCargoDetails(cargoList, container, false, trainGuid, stationGuid);

        cargoPanel.transform.Find("TrainCargoButton").GetComponent<CargoTabButton>().setTrainAndStationGameObj(train, station);
        cargoPanel.transform.Find("StationCargoButton").GetComponent<CargoTabButton>().setTrainAndStationGameObj(train, station);
        cargoPanel.transform.Find("YardCargoButton").GetComponent<CargoTabButton>().setTrainAndStationGameObj(train, station);
        cargoPanel.transform.Find("DepartButton").GetComponent<ButtonTrainDepart>().setTrainToDepart(train);
    }


    private void loadTrainOnlyCargoPanel(GameObject cargoPanel, GameObject train)
    {
        Guid trainGuid = train.GetComponent<TrainManager>().trainGUID;
        Transform container = getCargoContainer(cargoPanel);
        List<Cargo> trainCargoList = logicMgr.getTrainCargoList(trainGuid);
        showCargoDetails(trainCargoList, container, true, trainGuid, Guid.Empty);
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
    /// By default, the call to get the station cargo returns both (new) station cargo and also yard cargo
    /// This functions serves to return the sub-category of the cargo
    /// </summary>
    /// <returns>Either the station cargo or the yard cargo</returns>
    private List<Cargo> getStationSubCargo(List<Cargo> allStationCargo, bool getStation)
    {
        List<Cargo> output = new List<Cargo>();
        foreach (Cargo cargo in allStationCargo)
        {
            CargoAssociation cargoAssoc = cargo.CargoAssoc;
            if (getStation && cargoAssoc == CargoAssociation.STATION) // Get Station-Only cargo
            {
                output.Add(cargo);
            } 
            else if (!getStation && cargoAssoc == CargoAssociation.YARD)// Get Yard-Only cargo
            {
                output.Add(cargo);
            }
            else continue;
        }
        return output;
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
    private void showCargoDetails(List<Cargo> cargoList, Transform container, bool disableCargoButton, Guid trainguid, Guid stationguid)
    {
        foreach(Cargo cargo in cargoList)
        {
            GameObject cargoDetailButton = Instantiate(cargoCellPrefab);
            cargoDetailButton.transform.SetParent(container);
            cargoDetailButton.GetComponent<CargoDetailButton>().setCargoCellInformation(cargo, trainguid, stationguid);

            if (disableCargoButton)
            {
                cargoDetailButton.GetComponent<Button>().enabled = false;
            }

            Guid destStationGUID = cargo.TravelPlan.DestinationStation;
            string dest = logicMgr.getIndividualStationInfo(destStationGUID).Name;
            string cargoType = cargo.Type.ToString();
            string weight = ((int)(cargo.Weight)).ToString();
            string cargoDetail = cargoType + " (" + weight + " t)";

            CurrencyManager currMgr = cargo.CurrencyManager;
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
