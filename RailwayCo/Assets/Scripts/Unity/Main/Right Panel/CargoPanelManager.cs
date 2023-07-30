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

    private GameObject _train;
    private Guid _trainGuid;

    private void Awake()
    {
        if (!_cargoDetailButtonPrefab) Debug.LogError("Cargo Detail Button Prefab not found");

        GameObject lgMgr = GameObject.Find("LogicManager");
        if (!lgMgr) Debug.LogError("Unable to find the Logic Manager");
        _logicMgr = lgMgr.GetComponent<LogicManager>();
        if (!_logicMgr) Debug.LogError("Unable to find the Logic Manager Script");
    }

    public void SetupCargoPanel(GameObject train, GameObject platform)
    {
        _cargoPanel = this.gameObject;

        _train = train;
        if (!_train)
            _trainGuid = default;
        else
            _trainGuid = _train.GetComponent<TrainManager>().TrainGUID;

        _platform = platform;
        if (!_platform)
            _stationGuid = default;
        else
            _stationGuid = _platform.GetComponent<PlatformManager>().StationGUID;
    }

    public bool IsSameTrainOrPlatform(GameObject train, GameObject platform)
    {
        Guid trainGuid = default;
        if (train)
            trainGuid = train.GetComponent<TrainManager>().TrainGUID;

        Guid stationGuid = default;
        if (platform)
            stationGuid = platform.GetComponent<PlatformManager>().StationGUID;

        return _trainGuid == trainGuid || _stationGuid == stationGuid;
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
        StartCoroutine(UpdateTrainStats(trainStats));

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
            Debug.LogError("Unable to find the Content container");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return null;
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
            cargoDetailButton.GetComponent<CargoDetailButton>().SetCargoInformation(this, cargo, disableButton);
        }
    }

    private void UpdateTabCapacitySliders()
    {
        // Regardless of the Cargo Panel chosen, the GameObject that contains 
        // the tab buttons should be of this hierarchy:
        /// (Chosen Cargo Panel)
        ///     `-- Tabs
        ///         `-- TrainCargoButton
        ///         `-- StationCargoButton
        ///         `-- YardCargoButton
        try
        {
            Transform tabs = _cargoPanel.transform.Find("Tabs");
            tabs.Find("TrainCargoButton").GetComponent<CargoTabButton>().UpdateCapacity();
            tabs.Find("StationCargoButton").GetComponent<CargoTabButton>().UpdateCapacity();
            tabs.Find("YardCargoButton").GetComponent<CargoTabButton>().UpdateCapacity();
        }
        catch (NullReferenceException)
        {
            Debug.LogError("Unable to find the Tabs button container");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    ////////////////////////////////////////////////////
    // BACKEND FUNCTIONS
    ////////////////////////////////////////////////////

    public List<Cargo> GetTrainCargoList() => _logicMgr.GetTrainCargoList(_trainGuid);

    public List<Cargo> GetStationCargoList() => _logicMgr.GetStationCargoList(_stationGuid);

    public List<Cargo> GetYardCargoList() => _logicMgr.GetYardCargoList(_stationGuid);

    public IntAttribute GetTrainCapacity() => _logicMgr.GetTrainAttribute(_trainGuid).Capacity;

    public IntAttribute GetYardCapacity() => _logicMgr.GetStationAttribute(_stationGuid).YardCapacity;

    public bool MoveCargoBetweenTrainAndStation(Cargo cargo)
    {
        // Moving cargo ability should only be available when the cargo is in
        // the platform's associated station with a train inside.
        if (_trainGuid == Guid.Empty || _stationGuid == Guid.Empty)
            return false;
        bool result = _logicMgr.MoveCargoBetweenTrainAndStation(cargo, _trainGuid, _stationGuid);
        if (result)
            UpdateTabCapacitySliders();
        return result;
    }

    public DoubleAttribute GetTrainFuel() => _logicMgr.GetTrainAttribute(_trainGuid).Fuel;

    public DoubleAttribute GetTrainDurability() => _logicMgr.GetTrainAttribute(_trainGuid).Durability;

    public int GetStationNum(Guid stationGuid) => _logicMgr.GetIndividualStation(stationGuid).Number;

    ////////////////////////////////////////////////////
    // TRAIN STATS FUNCTIONS
    ////////////////////////////////////////////////////

    // Read from backend instead of using TrainMovement's TrainAttribute as the fuel and durability is updated
    // directly with backend and not utilised by TrainMovement
    private void PopulateTrainStats(Transform trainStats)
    {
        DoubleAttribute fuel = GetTrainFuel();
        Slider fuelSlider = trainStats.Find("Fuel").Find("FuelBar").GetComponent<Slider>();
        Image fuelBackground = fuelSlider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
        float newValue = (float)(fuel.Amount / fuel.UpperLimit);
        fuelSlider.value = newValue;
        fuelBackground.color = SliderGradient.GetColorDecremental(newValue);

        DoubleAttribute durability = GetTrainDurability();
        Slider durabilitySlider = trainStats.Find("Durability").Find("DurabilityBar").GetComponent<Slider>();
        Image durabilityBackground = durabilitySlider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
        newValue = (float)(durability.Amount / durability.UpperLimit);
        durabilitySlider.value = newValue;
        durabilityBackground.color = SliderGradient.GetColorDecremental(newValue);
    }

    private IEnumerator UpdateTrainStats(Transform trainStats)
    {
        while (true)
        {
            PopulateTrainStats(trainStats);
            yield return new WaitForSeconds(5);
        }
    }
}
