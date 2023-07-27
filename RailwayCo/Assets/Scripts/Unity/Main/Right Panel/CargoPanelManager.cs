using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject _cargoDetailButtonPrefab;

    private LogicManager _logicMgr;
    private GameObject _cargoPanel;

    private GameObject _platform;
    private Guid _stationGuid;
    private int _stationNum;

    private GameObject _train;
    private Guid _trainGuid;

    private void Awake()
    {
        if (!_cargoDetailButtonPrefab) Debug.LogError("Cargo Detail Button Prefab not found");

        GameObject lgMgr = GameObject.Find("LogicManager");
        if (!lgMgr) Debug.LogError("Unable to find the Logic Manager");
        _logicMgr = lgMgr.GetComponent<LogicManager>();
        if (!_logicMgr) Debug.LogError("Unable to find the Logic Manager Script");

        _cargoPanel = this.gameObject;
    }

    public static GameObject Init(GameObject cargoPanelPrefab, GameObject train, GameObject platform)
    {
        GameObject cargoPanel = Instantiate(cargoPanelPrefab);
        CargoPanelManager cargoPanelMgr = cargoPanel.GetComponent<CargoPanelManager>();
        cargoPanelMgr.SetupTrainAndPlatform(train, platform);
        return cargoPanel;
    }

    private void SetupTrainAndPlatform(GameObject train, GameObject platform)
    {
        _train = train;
        _trainGuid = _train.GetComponent<TrainManager>().TrainGUID;

        _platform = platform;
        _stationGuid = _platform.GetComponent<PlatformManager>().StationGUID;

        if (_stationGuid != default)
            _stationNum = _logicMgr.GetIndividualStation(_stationGuid).Number;
        else
            _stationNum = 0;
    }

    private void ResetCargoPanel()
    {
        Transform cargoContentContainer = GetCargoContainer();
        foreach (Transform child in cargoContentContainer)
        {
            Destroy(child.gameObject);
        }
    }


    /////////////////////////////////////////////////////
    // CARGO RENDERING OPTIONS
    ////////////////////////////////////////////////////

    public void PopulateCargoPanel(CargoTabOptions cargoTabOptions)
    {
        ResetCargoPanel();

        if (_train != null && _platform == null) // When the selected train is not in the platform
            PopulateTrainOnlyCargoPanel();
        else if (_train == null && _platform != null) // When the selected platform has no train
            PopulateStationOnlyCargoPanel();
        else if (_train != null && _platform != null)
            PopulateUnifiedCargoPanel(cargoTabOptions);
    }

    private void PopulateTrainOnlyCargoPanel()
    {
        if (_trainGuid == Guid.Empty)
        {
            Debug.LogError($"{_train.name} has an invalid GUID");
            return;
        }
        List<Cargo> trainCargoList = GetTrainCargoList();
        ShowCargoDetails(trainCargoList, true);

        _cargoPanel.transform.Find("TrainMetaInfo").Find("TrainName").GetComponent<Text>().text = _train.name;
    }

    private void PopulateStationOnlyCargoPanel()
    {
        if (_stationGuid == Guid.Empty)
        {
            Debug.LogError($"{_platform.name} has an invalid Station GUID");
            return;
        }

        List<Cargo> yardCargoList = GetYardCargoList();
        ShowCargoDetails(yardCargoList, true);

        _cargoPanel.transform.Find("CurrentStationName").Find("StationName").GetComponent<Text>().text = _platform.name;
        Transform bottomContainer = _cargoPanel.transform.Find("BottomContainer");
        bottomContainer.Find("AddTrainButton").GetComponent<PlatformTrainAddition>().UpdatePlatformInfo(_platform);
    }

    private void PopulateUnifiedCargoPanel(CargoTabOptions cargoTabOptions)
    {
        if (_trainGuid == Guid.Empty)
        {
            Debug.LogError($"{_train.name} has an invalid GUID");
            return;
        }
        if (_stationGuid == Guid.Empty)
        {
            Debug.LogError($"{_platform.name} has an invalid Station GUID");
            return;
        }

        List<Cargo> cargoList;
        switch (cargoTabOptions)
        {
            case CargoTabOptions.Nil:
            case CargoTabOptions.StationCargo:
                cargoList = GetStationCargoList();
                break;
            case CargoTabOptions.YardCargo:
                cargoList = GetYardCargoList();
                break;
            case CargoTabOptions.TrainCargo:
                cargoList = GetTrainCargoList();
                break;
            default:
                Debug.LogWarning("This block should never reach.");
                cargoList = new();
                break;
        }
        ShowCargoDetails(cargoList, false);

        _cargoPanel.transform.Find("CurrentStationName").Find("StationName").GetComponent<Text>().text = _platform.name;

        Transform bottomContainer = _cargoPanel.transform.Find("BottomContainer");

        Transform trainStats = bottomContainer.Find("TrainStats");
        StartCoroutine(UpdateTrainStats(_train, trainStats));

        Transform departBtns = bottomContainer.Find("DepartButtons");
        departBtns.Find("LeftDepartButton").GetComponent<TrainDepartButton>().SetTrainDepartInformation(_train, _platform);
        departBtns.Find("RightDepartButton").GetComponent<TrainDepartButton>().SetTrainDepartInformation(_train, _platform);
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
    private void ShowCargoDetails(List<Cargo> cargoList, bool disableButton)
    {
        Transform container = GetCargoContainer();
        if (!container) return;

        foreach (Cargo cargo in cargoList)
        {
            GameObject cargoDetailButton = Instantiate(_cargoDetailButtonPrefab);
            cargoDetailButton.transform.SetParent(container);
            cargoDetailButton.GetComponent<CargoDetailButton>().SetCargoInformation(cargo, _stationNum, disableButton);
        }
    }

    ////////////////////////////////////////////////////
    // BACKEND FUNCTIONS
    ////////////////////////////////////////////////////

    public List<Cargo> GetTrainCargoList() => _logicMgr.GetTrainCargoList(_trainGuid);

    public List<Cargo> GetStationCargoList() => _logicMgr.GetStationCargoList(_stationGuid);

    public List<Cargo> GetYardCargoList() => _logicMgr.GetYardCargoList(_stationGuid);

    public bool MoveCargoBetweenTrainAndStation(Cargo cargo)
    {
        // Moving cargo ability should only be available when the cargo is in
        // the platform's associated station with a train inside.
        if (_trainGuid == Guid.Empty || _stationGuid == Guid.Empty)
            return false;
        return _logicMgr.MoveCargoBetweenTrainAndStation(cargo, _trainGuid, _stationGuid);
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
