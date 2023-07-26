using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Only the Right Panel (under the MainUI) have this script component attached
// All other GameObejcts should find this manager via reference to the RightPanel GameObject.
public class RightPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject _cargoTrainStationPanelPrefab;
    [SerializeField] private GameObject _cargoStationOnlyPanelPrefab;
    [SerializeField] private GameObject _cargoTrainOnlyPanelPrefab;
    [SerializeField] private GameObject _FullVerticalSubPanelPrefab;
    [SerializeField] private GameObject _cargoDetailButtonPrefab;
    [SerializeField] private GameObject _trainDetailButtonPrefab;
    [SerializeField] private GameObject _platformDetailButtonPrefab;

    private RightPanelManagerBackend _rpBackend;
    private LogicManager _logicMgr;
    private CameraManager _camMgr;
    private float _rightPanelWidthRatio;

    // Only meaningful in the context of the CargoTrainStationPanel (the 3 buttons at the top), and only when the train is stopped at the platform's associated station
    // to show the correct association (either from the station, train or the yard)
    // Otherwise, it has no effect
    private CargoTabOptions _cargoTabOptions = CargoTabOptions.NIL;
    public enum CargoTabOptions
    {
        NIL,
        TRAIN_CARGO,
        STATION_CARGO,
        YARD_CARGO
    }

    // Set by CargoTabButton.cs. Used only in the case when train is in the platform's associated station
    public void UpdateChosenCargoTab(CargoTabOptions cargoOptions)
    {
        _cargoTabOptions = cargoOptions;
    }

    ////////////////////////////////////////////
    // INITIALISATION
    ////////////////////////////////////////////
    
    private void Awake()
    {
        GameObject lgMgr = GameObject.Find("LogicManager");
        if (!lgMgr) Debug.LogError("Unable to find the Logic Manager");
        _logicMgr = lgMgr.GetComponent<LogicManager>();
        if (!_logicMgr) Debug.LogError("Unable to find the Logic Manager Script");
        _camMgr = GameObject.Find("CameraList").GetComponent<CameraManager>();
        _rpBackend = this.GetComponent<RightPanelManagerBackend>();

        GameObject mainUI = GameObject.Find("MainUI");
        if (!mainUI) Debug.LogError("Main UI Not Found!");
        Vector2 refReso = mainUI.GetComponent<CanvasScaler>().referenceResolution;
        _rightPanelWidthRatio = this.GetComponent<RectTransform>().rect.width / refReso[0];

        if (!_cargoTrainStationPanelPrefab) Debug.LogError("Train Station Yard Cargo Panel Prefab not found");
        if (!_cargoStationOnlyPanelPrefab) Debug.LogError("Station Yard Cargo Panel Prefab not found");
        if (!_cargoTrainOnlyPanelPrefab) Debug.LogError("Train Yard Cargo Panel Prefab not found");
        if (!_FullVerticalSubPanelPrefab) Debug.LogError("Full Subpanel Prefab not found");
        if (!_cargoDetailButtonPrefab) Debug.LogError("Cargo Detail Button Prefab not found");
        if (!_trainDetailButtonPrefab) Debug.LogError("Train Detail Button Prefab not found");
        if (!_platformDetailButtonPrefab) Debug.LogError("Station Detail Button Prefab not found");

    }

    ///////////////////////////////////////////////
    // RIGHT PANEL MAINTENANCE
    //////////////////////////////////////////////

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseRightPanel();
        }
    }


    public void CloseRightPanel()
    {
        this.gameObject.SetActive(false);
        _camMgr.RightPanelInactivateCameraUpdate();
    }

    private void ResetRightPanel()
    {
        if (!this.gameObject.activeInHierarchy) this.gameObject.SetActive(true);
        DestroyRightPanelChildren();
    }


    private void DestroyRightPanelChildren()
    {
        int noChild = this.transform.childCount;
        for (int i = 0; i<noChild; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }

    private void AlignSubPanelAndUpdateCamera(GameObject subpanel, bool isTrainInPlatform)
    {
        subpanel.transform.SetParent(this.transform);
        subpanel.transform.localPosition = new Vector3(0, 0, 0);
        subpanel.transform.localScale = new Vector3(1, 1, 1);
        _camMgr.RightPanelActivateCameraUpdate(_rightPanelWidthRatio, isTrainInPlatform);
    }

    /////////////////////////////////////////////////////
    // CARGO RENDERING OPTIONS
    ////////////////////////////////////////////////////
    private void LoadTrainOnlyCargoPanel(GameObject cargoPanel, GameObject train)
    {
        Transform container = GetCargoContainer(cargoPanel);
        if (!container) return;

        Guid trainGuid = train.GetComponent<TrainManager>().TrainGUID;
        if (trainGuid == Guid.Empty)
        {
            Debug.LogError($"{train.name} has an invalid GUID");
            return;
        }
        List<Cargo> trainCargoList = _logicMgr.GetTrainCargoList(trainGuid);
        ShowCargoDetails(trainCargoList, container, true, trainGuid, Guid.Empty);

        cargoPanel.transform.Find("TrainMetaInfo").Find("TrainName").GetComponent<Text>().text = train.name;
    }

    private void LoadStationOnlyCargoPanel(GameObject cargoPanel, GameObject platform)
    {
        Transform container = GetCargoContainer(cargoPanel);
        if (!container) return;

        Guid stationGuid = platform.GetComponent<PlatformManager>().StationGUID;
        if (stationGuid == Guid.Empty)
        {
            Debug.LogError($"{platform.name} has an invalid Station GUID");
            return;
        }

        List<Cargo> yardCargoList = _logicMgr.GetSelectedStationCargoList(stationGuid, false);
        ShowCargoDetails(yardCargoList, container, true, Guid.Empty, stationGuid);

        cargoPanel.transform.Find("CurrentStationName").Find("StationName").GetComponent<Text>().text = platform.name;
        Transform bottomContainer = cargoPanel.transform.Find("BottomContainer");
        bottomContainer.Find("AddTrainButton").GetComponent<PlatformTrainAddition>().UpdatePlatformInfo(platform);
    }

    private void LoadUnifiedCargoPanel(GameObject cargoPanel, GameObject train, GameObject platform)
    {
        Transform container = GetCargoContainer(cargoPanel);
        if (!container) return;

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
        switch (_cargoTabOptions)
        {
            case CargoTabOptions.NIL:
            case CargoTabOptions.STATION_CARGO:
                cargoList = _logicMgr.GetSelectedStationCargoList(stationGuid, true);
                break;
            case CargoTabOptions.YARD_CARGO:
                cargoList = _logicMgr.GetSelectedStationCargoList(stationGuid, false);
                break;
            case CargoTabOptions.TRAIN_CARGO:
                cargoList = _logicMgr.GetTrainCargoList(trainGuid);
                break;
            default:
                Debug.LogWarning("This block should never reach.");
                cargoList = new();
                break;

        }
        ShowCargoDetails(cargoList, container, false, trainGuid, stationGuid);

        cargoPanel.transform.Find("CurrentStationName").Find("StationName").GetComponent<Text>().text = platform.name;

        Transform bottomContainer = cargoPanel.transform.Find("BottomContainer");
        
        Transform trainStats = bottomContainer.Find("TrainStats");
        StartCoroutine(UpdateTrainStats(train, trainStats));

        Transform departBtns = bottomContainer.Find("DepartButtons");
        departBtns.Find("LeftDepartButton").GetComponent<TrainDepartButton>().SetTrainDepartInformation(train, platform);
        departBtns.Find("RightDepartButton").GetComponent<TrainDepartButton>().SetTrainDepartInformation(train, platform);

        Transform tabs = cargoPanel.transform.Find("Tabs");
        tabs.Find("TrainCargoButton").GetComponent<CargoTabButton>().SetCargoTabButtonInformation(train, platform);
        tabs.Find("StationCargoButton").GetComponent<CargoTabButton>().SetCargoTabButtonInformation(train, platform);
        tabs.Find("YardCargoButton").GetComponent<CargoTabButton>().SetCargoTabButtonInformation(train, platform);
    }


    //////////////////////////////////////////////////
    // CARGO BACKEND PROCESSES
    //////////////////////////////////////////////////

    private Transform GetCargoContainer(GameObject cargoPanel)
    {
        // Regardless of the Cargo Panel chosen, the subpanel that contains the container for the cargo should be of this hirarchy
        /// (Chosen Cargo Panel)
        ///     `-- CargoContentPanel
        ///         `-- View
        ///             `-- Content
        try
        {
            return cargoPanel.transform.Find("CargoContentPanel").Find("View").Find("Content");
        }
        catch (NullReferenceException)
        {
            Debug.LogError("Unable to find and the cargo panel's container");
            return null;
        }
        catch (Exception)
        {
            Debug.LogError("Unhandled Exception in getCargoContainer");
            return null;
        }
    }
    
  

    /// <summary>
    /// Renders the list of cargo associated with the train and/or station
    /// </summary>
    /// <param name="cargoList"> List of Cargo associated with any GameObject that is supposed to hold such info </param>
    /// <param name="container">

    /// </param>
    private void ShowCargoDetails(List<Cargo> cargoList, Transform container, bool disableCargoDetailButton, Guid trainguid, Guid stationguid)
    {
        foreach(Cargo cargo in cargoList)
        {
            GameObject cargoDetailButton = Instantiate(_cargoDetailButtonPrefab);
            cargoDetailButton.transform.SetParent(container);
            cargoDetailButton.GetComponent<CargoDetailButton>().SetCargoInformation(cargo, trainguid, stationguid, disableCargoDetailButton);
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


    ////////////////////////////////////////////////////
    // PUBLIC FUNCTIONS
    ////////////////////////////////////////////////////

    // Loads the cargo panel, Main entrypoint that determines what gets rendered
    public void LoadCargoPanel(GameObject train, GameObject platform)
    {
        ResetRightPanel();
        GameObject cargoPanel;
        bool trainInPlatform = false;

        if (train != null && platform == null) // When the selected train is not in the platform
        {
            cargoPanel = Instantiate(_cargoTrainOnlyPanelPrefab);
            LoadTrainOnlyCargoPanel(cargoPanel, train);
        }
        else if (train == null && platform != null) // When the selected platform has no train
        {
            cargoPanel = Instantiate(_cargoStationOnlyPanelPrefab);
            LoadStationOnlyCargoPanel(cargoPanel, platform);
        }
        else if (train != null && platform != null)
        {
            cargoPanel = Instantiate(_cargoTrainStationPanelPrefab);
            LoadUnifiedCargoPanel(cargoPanel, train, platform);
            trainInPlatform = true;
        }
        else
        {
            Debug.LogWarning("This should never happen! At least Either the train or the staion must be valid");
            ResetRightPanel();
            return;
        }

        if (cargoPanel)
        {
            AlignSubPanelAndUpdateCamera(cargoPanel, trainInPlatform);
        }
    }

    public void LoadTrainList()
    {
        ResetRightPanel();

        GameObject rightSubPanel = Instantiate(_FullVerticalSubPanelPrefab);
        Transform container = rightSubPanel.transform.Find("Container");

        GameObject[] trainList = GameObject.FindGameObjectsWithTag("Train");
        for (int i = 0; i < trainList.Length; i++)
        {
            GameObject trainGO = trainList[i];
            // TODO: Display durability and fuel stats in TrainDetailButton
            // TrainManager trainManager = trainGO.GetComponent<TrainManager>();
            // Train train = _logicMgr.GetTrainClassObject(trainManager.TrainGUID);

            GameObject trainDetailButton = Instantiate(_trainDetailButtonPrefab);
            trainDetailButton.transform.SetParent(container);
            trainDetailButton.GetComponent<TrainDetailButton>().SetTrainGameObject(trainGO);
        }
        AlignSubPanelAndUpdateCamera(rightSubPanel, false);
    }

    public void LoadPlatformList()
    {
        ResetRightPanel();

        GameObject rightSubPanel = Instantiate(_FullVerticalSubPanelPrefab);
        Transform container = rightSubPanel.transform.Find("Container");


        List<GameObject> platformList = _rpBackend.GetPlatformListByUnlockStatus();

        for (int i = 0; i < platformList.Count; i++)
        {
            GameObject platformDetailButton = Instantiate(_platformDetailButtonPrefab);
            platformDetailButton.transform.SetParent(container);
            platformDetailButton.GetComponent<PlatformDetailButton>().SetPlatformGameObject(platformList[i]);
        }
        AlignSubPanelAndUpdateCamera(rightSubPanel, false);
    }
}
