﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject _cargoTrainStationPanel;
    [SerializeField] private GameObject _cargoStationOnlyPanel;
    [SerializeField] private GameObject _cargoTrainOnlyPanel;
    [SerializeField] private GameObject _cargoDetailButtonPrefab;

    private GameObject _cargoPanel;
    private GameObject _platform;
    private Guid _stationGuid;
    private Guid _platformGuid;
    private GameObject _train;
    private Guid _trainGuid;

    private void Awake()
    {
        if (!_cargoTrainStationPanel) Debug.LogError("Train Station Yard Cargo Panel not found");
        if (!_cargoStationOnlyPanel) Debug.LogError("Station Yard Cargo Panel not found");
        if (!_cargoTrainOnlyPanel) Debug.LogError("Train Yard Cargo Panel not found");
        if (!_cargoDetailButtonPrefab) Debug.LogError("Cargo Detail Button Prefab not found");
    }

    private void OnEnable()
    {
        try
        {
            if (!_cargoPanel) return;
            UpdateTabCapacitySliders();
        }
        catch (Exception)
        {
            Debug.Log("Train and station info not available yet");
        }
    }

    public bool SetupCargoPanel(Guid trainGuid, Guid platformGuid)
    {
        DeactivateActiveCargoPanel();

        _train = TrainManager.GetGameObject(trainGuid);
        _trainGuid = trainGuid;

        _platform = PlatformManager.GetGameObject(platformGuid);
        _stationGuid = default;
        _platformGuid = platformGuid;
        if (_platform)
            _stationGuid = _platform.GetComponent<PlatformController>().StationGuid;

        if (_train && !_platform) // When the selected train is not in the platform
            _cargoPanel = _cargoTrainOnlyPanel;
        else if (!_train && _platform) // When the selected platform has no train
            _cargoPanel = _cargoStationOnlyPanel;
        else if (_train && _platform)
            _cargoPanel = _cargoTrainStationPanel;
        else
        {
            Debug.LogWarning("This should never happen! At least Either the train or the station must be valid");
            return false;
        }
        
        _cargoPanel.SetActive(true);
        return true;
    }

    public bool IsSameTrainOrPlatform(Guid trainGuid, Guid platformGuid)
    {
        return _trainGuid == trainGuid || _platformGuid == platformGuid;
    }

    private void ResetCargoPanel()
    {
        Transform cargoContentContainer = GetCargoContainer();
        foreach (Transform child in cargoContentContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void DeactivateActiveCargoPanel()
    {
        if (_cargoPanel)
        {
            _cargoPanel.SetActive(false);
            _cargoPanel = null;
        }
    }


    /////////////////////////////////////////////////////
    // CARGO RENDERING OPTIONS
    ////////////////////////////////////////////////////

    public void PopulateCargoPanel(CargoTabOptions cargoTabOptions)
    {
        ResetCargoPanel();

        if (_train && !_platform) // When the selected train is not in the platform
            PopulateTrainOnlyCargoPanel();
        else if (!_train && _platform) // When the selected platform has no train
            PopulateStationOnlyCargoPanel();
        else if (_train && _platform)
            PopulateUnifiedCargoPanel(cargoTabOptions);
    }

    private void PopulateTrainOnlyCargoPanel()
    {
        if (_trainGuid == Guid.Empty)
        {
            Debug.LogError($"{_train.name} has an invalid Train GUID");
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

    public List<Cargo> GetTrainCargoList() => CargoManager.GetTrainCargoList(_trainGuid);

    public List<Cargo> GetStationCargoList() => CargoManager.GetStationCargoList(_stationGuid);

    public List<Cargo> GetYardCargoList() => CargoManager.GetYardCargoList(_stationGuid);

    public IntAttribute GetTrainCapacity() => TrainManager.GetTrainAttribute(_trainGuid).Capacity;

    public IntAttribute GetYardCapacity() => PlatformManager.GetStationAttribute(_stationGuid).YardCapacity;

    public DoubleAttribute GetTrainFuel() => TrainManager.GetTrainAttribute(_trainGuid).Fuel;

    public DoubleAttribute GetTrainDurability() => TrainManager.GetTrainAttribute(_trainGuid).Durability;

    public int GetStationNum(Guid stationGuid) => PlatformManager.GetStationClassObject(stationGuid).Number;

    public bool MoveCargoBetweenTrainAndStation(Cargo cargo)
    {
        // Moving cargo ability should only be available when the cargo is in
        // the platform's associated station with a train inside.
        if (_trainGuid == Guid.Empty || _stationGuid == Guid.Empty)
            return false;
        bool result = CargoManager.MoveCargoBetweenTrainAndStation(cargo, _trainGuid, _stationGuid);
        if (result)
            UpdateTabCapacitySliders();
        return result;
    }

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
        for (; ; )
        {
            PopulateTrainStats(trainStats);
            yield return new WaitForSeconds(5);
        }
    }

    public void OnHoverFuelEnter()
    {
        DoubleAttribute fuel = GetTrainFuel();
        double current = fuel.Amount;
        double total = fuel.UpperLimit;
        TooltipManager.Show($"{current} / {total}", "Fuel");
    }

    public void OnHoverDurabilityEnter()
    {
        DoubleAttribute durability = GetTrainDurability();
        double current = durability.Amount;
        double total = durability.UpperLimit;
        TooltipManager.Show($"{current} / {total}", "Durability");
    }

    public void OnHoverTrainStatsExit() => TooltipManager.Hide();
}
