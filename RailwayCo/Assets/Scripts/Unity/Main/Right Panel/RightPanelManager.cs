using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Only the Right Panel (under the MainUI) have this script component attached
// All other GameObejcts should find this manager via reference to the RightPanel GameObject.
public class RightPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject _cargoTrainStationPanel;
    [SerializeField] private GameObject _cargoStationOnlyPanel;
    [SerializeField] private GameObject _cargoTrainOnlyPanel;
    [SerializeField] private GameObject _FullVerticalSubPanel;
    [SerializeField] private GameObject _trainDetailButtonPrefab;
    [SerializeField] private GameObject _platformDetailButtonPrefab;

    private CameraManager _camMgr;
    private GameObject _subPanel;
    private RightPanelType _activeRightPanelType;
    private float _rightPanelWidthRatio;

    ////////////////////////////////////////////
    // INITIALISATION
    ////////////////////////////////////////////
    
    private void Awake()
    {
        _camMgr = GameObject.Find("CameraList").GetComponent<CameraManager>();

        GameObject mainUI = GameObject.Find("MainUI");
        if (!mainUI) Debug.LogError("Main UI Not Found!");
        Vector2 refReso = mainUI.GetComponent<CanvasScaler>().referenceResolution;
        _rightPanelWidthRatio = GetComponent<RectTransform>().rect.width / refReso[0];

        if (!_cargoTrainStationPanel) Debug.LogError("Train Station Yard Cargo Panel not found");
        if (!_cargoStationOnlyPanel) Debug.LogError("Station Yard Cargo Panel not found");
        if (!_cargoTrainOnlyPanel) Debug.LogError("Train Yard Cargo Panel not found");
        if (!_FullVerticalSubPanel) Debug.LogError("Full Subpanel not found");
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
        _camMgr.RightPanelInactivateCameraUpdate();
        DeactivateActiveSubPanel();
        gameObject.SetActive(false);
    }

    private void ResetRightPanel()
    {
        gameObject.SetActive(true);
        if (_subPanel)
        {
            if (_activeRightPanelType == RightPanelType.Train || _activeRightPanelType == RightPanelType.Platform)
            {
                Transform container = _subPanel.transform.Find("Container");
                foreach (Transform child in container)
                {
                    Destroy(child.gameObject);
                }
            }
            else if (_activeRightPanelType != RightPanelType.Cargo)
                Debug.LogWarning("Unhandled RightPanelType in ResetRightPanel");
            // Cargo RightPanelType will reset on its own before each use
        }
    }

    private void DeactivateActiveSubPanel()
    {
        if (_subPanel)
        {
            _subPanel.SetActive(false);
            _subPanel = null;
        }
    }

    private void AlignSubPanelAndUpdateCamera(bool isTrainInPlatform)
    {
        _subPanel.transform.SetParent(transform);
        _subPanel.transform.localPosition = new Vector3(0, 0, 0);
        _subPanel.transform.localScale = new Vector3(1, 1, 1);
        _camMgr.RightPanelActivateCameraUpdate(_rightPanelWidthRatio, isTrainInPlatform);
    }

    private void UpdateCamera(bool isTrainInPlatform)
    {
        _camMgr.RightPanelActivateCameraUpdate(_rightPanelWidthRatio, isTrainInPlatform);
    }


    ////////////////////////////////////////////////////
    // BACKEND FUNCTIONS
    ////////////////////////////////////////////////////

    private List<GameObject> GetPlatformListByUnlockStatus()
    {
        List<GameObject> collection = new();
        collection.AddRange(GameObject.FindGameObjectsWithTag("PlatformLR"));
        collection.AddRange(GameObject.FindGameObjectsWithTag("PlatformTD"));
        return collection.OrderByDescending(platform => platform.GetComponent<PlatformController>().IsPlatformUnlocked)
                         .ToList();
    }

    private void SetupSubPanel(RightPanelType rightPanelType)
    {
        _subPanel.SetActive(true);
        _activeRightPanelType = rightPanelType;
    }

    private void SetupCargoPanel(GameObject train, GameObject platform, CargoTabOptions cargoTabOptions)
    {
        SetupSubPanel(RightPanelType.Cargo);

        CargoPanelManager cargoPanelMgr = _subPanel.GetComponent<CargoPanelManager>();
        cargoPanelMgr.SetupCargoPanel(train, platform);

        bool trainInPlatform = train != null && platform != null;
        UpdateCamera(trainInPlatform);
        cargoPanelMgr.PopulateCargoPanel(cargoTabOptions);
    }

    ////////////////////////////////////////////////////
    // PUBLIC FUNCTIONS
    ////////////////////////////////////////////////////

    public bool IsActivePanelSamePanelType(RightPanelType rightPanelType)
    {
        return _activeRightPanelType == rightPanelType;
    }

    public bool IsActiveCargoPanelSameTrainOrPlatform(GameObject train, GameObject platform)
    {
        if (_activeRightPanelType != RightPanelType.Cargo)
            return false;

        CargoPanelManager cargoPanelMgr = _subPanel.GetComponent<CargoPanelManager>();
        return cargoPanelMgr.IsSameTrainOrPlatform(train, platform);
    }

    public CargoPanelManager GetCargoPanelManager()
    {
        if (_activeRightPanelType != RightPanelType.Cargo || !_subPanel)
            return default;
        return _subPanel.GetComponent<CargoPanelManager>();
    }

    // Loads the cargo panel, Main entrypoint that determines what gets rendered
    public bool LoadCargoPanel(GameObject train, GameObject platform, CargoTabOptions cargoTabOptions)
    {
        DeactivateActiveSubPanel();
        ResetRightPanel();

        if (train != null && platform == null) // When the selected train is not in the platform
            _subPanel = _cargoTrainOnlyPanel;
        else if (train == null && platform != null) // When the selected platform has no train
            _subPanel = _cargoStationOnlyPanel;
        else if (train != null && platform != null)
            _subPanel = _cargoTrainStationPanel;
        else
        {
            Debug.LogWarning("This should never happen! At least Either the train or the station must be valid");
            return false;
        }

        SetupCargoPanel(train, platform, cargoTabOptions);
        return true;
    }

    public void LoadTrainList()
    {
        DeactivateActiveSubPanel();
        _subPanel = _FullVerticalSubPanel;
        SetupSubPanel(RightPanelType.Train);
        ResetRightPanel();
        Transform container = _subPanel.transform.Find("Container");

        List<GameObject> trainList = GameObject.FindGameObjectsWithTag("Train").ToList();
        foreach (GameObject trainGO in trainList)
        {
            // TODO: Display durability and fuel stats in TrainDetailButton
            // TrainManager trainManager = trainGO.GetComponent<TrainManager>();
            // Train train = _logicMgr.GetTrainClassObject(trainManager.TrainGUID);

            GameObject trainDetailButton = Instantiate(_trainDetailButtonPrefab);
            trainDetailButton.transform.SetParent(container);
            trainDetailButton.GetComponent<TrainDetailButton>().SetTrainGameObject(trainGO);
        }
        AlignSubPanelAndUpdateCamera(false);
    }

    public void LoadPlatformList()
    {
        DeactivateActiveSubPanel();
        _subPanel = _FullVerticalSubPanel;
        SetupSubPanel(RightPanelType.Platform);
        ResetRightPanel();
        Transform container = _subPanel.transform.Find("Container");

        List<GameObject> platformList = GetPlatformListByUnlockStatus();
        for (int i = 0; i < platformList.Count; i++)
        {
            GameObject platformDetailButton = Instantiate(_platformDetailButtonPrefab);
            platformDetailButton.transform.SetParent(container);
            platformDetailButton.GetComponent<PlatformDetailButton>().SetPlatformGameObject(platformList[i]);
        }
        AlignSubPanelAndUpdateCamera(false);
    }
}
