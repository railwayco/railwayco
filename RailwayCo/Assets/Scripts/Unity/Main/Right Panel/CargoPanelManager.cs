using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject _cargoDetailButtonPrefab;

    private RightPanelManager _rpMgr;
    private GameObject _cargoPanel;


    private void Awake()
    {
        if (!_cargoDetailButtonPrefab) Debug.LogError("Cargo Detail Button Prefab not found");
        
        GameObject mainUI = GameObject.Find("MainUI");
        if (!mainUI) Debug.LogError("Main UI Not Found!");

        Transform rightPanel = mainUI.transform.Find("RightPanel");
        if (rightPanel) _rpMgr = rightPanel.GetComponent<RightPanelManager>();

        _cargoPanel = this.gameObject;
    }


    /////////////////////////////////////////////////////
    // CARGO RENDERING OPTIONS
    ////////////////////////////////////////////////////

    public void PopulateCargoPanel(GameObject train, GameObject platform, CargoTabOptions cargoTabOptions)
    {
        if (train != null && platform == null) // When the selected train is not in the platform
            PopulateTrainOnlyCargoPanel(train);
        else if (train == null && platform != null) // When the selected platform has no train
            PopulateStationOnlyCargoPanel(platform);
        else if (train != null && platform != null)
            PopulateUnifiedCargoPanel(train, platform, cargoTabOptions);
    }

    private void PopulateTrainOnlyCargoPanel(GameObject train)
    {
        Guid trainGuid = train.GetComponent<TrainManager>().TrainGUID;
        if (trainGuid == Guid.Empty)
        {
            Debug.LogError($"{train.name} has an invalid GUID");
            return;
        }
        List<Cargo> trainCargoList = _rpMgr.GetTrainCargoList(trainGuid);
        ShowCargoDetails(trainCargoList, true, trainGuid, Guid.Empty);

        _cargoPanel.transform.Find("TrainMetaInfo").Find("TrainName").GetComponent<Text>().text = train.name;
    }

    private void PopulateStationOnlyCargoPanel(GameObject platform)
    {
        Guid stationGuid = platform.GetComponent<PlatformManager>().StationGUID;
        if (stationGuid == Guid.Empty)
        {
            Debug.LogError($"{platform.name} has an invalid Station GUID");
            return;
        }

        List<Cargo> yardCargoList = _rpMgr.GetYardCargoList(stationGuid);
        ShowCargoDetails(yardCargoList, true, Guid.Empty, stationGuid);

        _cargoPanel.transform.Find("CurrentStationName").Find("StationName").GetComponent<Text>().text = platform.name;
        Transform bottomContainer = _cargoPanel.transform.Find("BottomContainer");
        bottomContainer.Find("AddTrainButton").GetComponent<PlatformTrainAddition>().UpdatePlatformInfo(platform);
    }

    private void PopulateUnifiedCargoPanel(GameObject train, GameObject platform, CargoTabOptions cargoTabOptions)
    {
        Guid trainGuid = train.GetComponent<TrainManager>().TrainGUID;
        if (trainGuid == Guid.Empty)
        {
            Debug.LogError($"{train.name} has an invalid GUID");
            return;
        }
        Guid stationGuid = platform.GetComponent<PlatformManager>().StationGUID;
        if (stationGuid == Guid.Empty)
        {
            Debug.LogError($"{platform.name} has an invalid Station GUID");
            return;
        }

        List<Cargo> cargoList;
        switch (cargoTabOptions)
        {
            case CargoTabOptions.Nil:
            case CargoTabOptions.StationCargo:
                cargoList = _rpMgr.GetStationCargoList(stationGuid);
                break;
            case CargoTabOptions.YardCargo:
                cargoList = _rpMgr.GetYardCargoList(stationGuid);
                break;
            case CargoTabOptions.TrainCargo:
                cargoList = _rpMgr.GetTrainCargoList(trainGuid);
                break;
            default:
                Debug.LogWarning("This block should never reach.");
                cargoList = new();
                break;
        }
        ShowCargoDetails(cargoList, false, trainGuid, stationGuid);

        _cargoPanel.transform.Find("CurrentStationName").Find("StationName").GetComponent<Text>().text = platform.name;

        Transform bottomContainer = _cargoPanel.transform.Find("BottomContainer");

        Transform trainStats = bottomContainer.Find("TrainStats");
        StartCoroutine(UpdateTrainStats(train, trainStats));

        Transform departBtns = bottomContainer.Find("DepartButtons");
        departBtns.Find("LeftDepartButton").GetComponent<TrainDepartButton>().SetTrainDepartInformation(train, platform);
        departBtns.Find("RightDepartButton").GetComponent<TrainDepartButton>().SetTrainDepartInformation(train, platform);

        Transform tabs = _cargoPanel.transform.Find("Tabs");
        tabs.Find("TrainCargoButton").GetComponent<CargoTabButton>().SetCargoTabButtonInformation(train, platform);
        tabs.Find("StationCargoButton").GetComponent<CargoTabButton>().SetCargoTabButtonInformation(train, platform);
        tabs.Find("YardCargoButton").GetComponent<CargoTabButton>().SetCargoTabButtonInformation(train, platform);
    }


    //////////////////////////////////////////////////
    // CARGO BACKEND PROCESSES
    //////////////////////////////////////////////////

    private Transform GetCargoContainer()
    {
        // Regardless of the Cargo Panel chosen, the subpanel that contains the container
        // for the cargo should be of this hierarchy:
        /// (Chosen Cargo Panel)
        ///     `-- CargoContentPanel
        ///         `-- View
        ///             `-- Content
        try
        {
            return _cargoPanel.transform.Find("CargoContentPanel").Find("View").Find("Content");
        }
        catch (NullReferenceException)
        {
            Debug.LogError("Unable to find the cargo panel's container");
            return null;
        }
        catch (Exception)
        {
            Debug.LogError("Unhandled Exception in GetCargoContainer");
            return null;
        }
    }


    /// <summary>
    /// Renders the list of cargo associated with the train and/or station
    /// </summary>
    /// <param name="cargoList"> List of Cargo associated with any GameObject that is supposed to hold such info </param>
    private void ShowCargoDetails(List<Cargo> cargoList, bool disableButton, Guid trainguid, Guid stationguid)
    {
        Transform container = GetCargoContainer();
        if (!container) return;

        foreach (Cargo cargo in cargoList)
        {
            GameObject cargoDetailButton = Instantiate(_cargoDetailButtonPrefab);
            cargoDetailButton.transform.SetParent(container);
            cargoDetailButton.GetComponent<CargoDetailButton>().SetCargoInformation(cargo, trainguid, stationguid, disableButton);
        }
    }

    ////////////////////////////////////////////////////
    // TRAIN STATS FUNCTIONS
    ////////////////////////////////////////////////////

    private void PopulateTrainStats(GameObject train, Transform trainStats)
    {
        TrainAttribute trainAttribute = train.GetComponent<TrainMovement>().TrainAttribute;

        DoubleAttribute fuel = trainAttribute.Fuel;
        Slider fuelSlider = trainStats.Find("Fuel").Find("FuelBar").GetComponent<Slider>();
        fuelSlider.value = (float)(fuel.Amount / fuel.UpperLimit);

        DoubleAttribute durability = trainAttribute.Durability;
        Slider durabilitySlider = trainStats.Find("Durability").Find("DurabilityBar").GetComponent<Slider>();
        durabilitySlider.value = (float)(durability.Amount / durability.UpperLimit);
    }

    private IEnumerator UpdateTrainStats(GameObject train, Transform trainStats)
    {
        while (true)
        {
            PopulateTrainStats(train, trainStats);
            yield return new WaitForSeconds(5);
        }
    }
}
