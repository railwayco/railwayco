using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RightPanelManager : MonoBehaviour
{
    private static RightPanelManager Instance { get; set; }
    public static bool IsActiveAndEnabled => Instance != null && Instance.isActiveAndEnabled;

    [SerializeField] private GameObject _cargoTrainStationPanel;
    [SerializeField] private GameObject _cargoStationOnlyPanel;
    [SerializeField] private GameObject _cargoTrainOnlyPanel;
    [SerializeField] private GameObject _FullVerticalSubPanel;
    [SerializeField] private GameObject _trainDetailButtonPrefab;
    [SerializeField] private GameObject _platformDetailButtonPrefab;

    private GameObject _subPanel;
    private RightPanelType _activeRightPanelType;
    private float _rightPanelWidthRatio;

    ////////////////////////////////////////////
    // INITIALISATION
    ////////////////////////////////////////////

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        CloseRightPanel();
        Instance.GetComponent<Image>().enabled = true;

        GameObject mainUI = GameObject.Find("MainUI");
        if (!mainUI) Debug.LogError("Main UI Not Found!");
        Vector2 refReso = mainUI.GetComponent<CanvasScaler>().referenceResolution;
        Instance._rightPanelWidthRatio = GetComponent<RectTransform>().rect.width / refReso[0];

        if (!Instance._cargoTrainStationPanel) Debug.LogError("Train Station Yard Cargo Panel not found");
        if (!Instance._cargoStationOnlyPanel) Debug.LogError("Station Yard Cargo Panel not found");
        if (!Instance._cargoTrainOnlyPanel) Debug.LogError("Train Yard Cargo Panel not found");
        if (!Instance._FullVerticalSubPanel) Debug.LogError("Full Subpanel not found");
        if (!Instance._trainDetailButtonPrefab) Debug.LogError("Train Detail Button Prefab not found");
        if (!Instance._platformDetailButtonPrefab) Debug.LogError("Station Detail Button Prefab not found");
    }

    ///////////////////////////////////////////////
    // RIGHT PANEL MAINTENANCE
    //////////////////////////////////////////////

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CloseRightPanel();
    }

    public static void CloseRightPanel()
    {
        if (!Instance.gameObject.activeInHierarchy) return;

        CameraManager.RightPanelInactivateCameraUpdate();
        DeactivateActiveSubPanel();
        Instance.gameObject.SetActive(false);
    }

    private static void ResetRightPanel()
    {
        Instance.gameObject.SetActive(true);
        if (Instance._subPanel)
        {
            if (Instance._activeRightPanelType == RightPanelType.Train ||
                Instance._activeRightPanelType == RightPanelType.Platform)
            {
                Transform container = Instance._subPanel.transform.Find("Container");
                foreach (Transform child in container)
                {
                    Destroy(child.gameObject);
                }
            }
            else if (Instance._activeRightPanelType != RightPanelType.Cargo)
                Debug.LogWarning("Unhandled RightPanelType in ResetRightPanel");
            // Cargo RightPanelType will reset on its own before each use
        }
    }

    private static void DeactivateActiveSubPanel()
    {
        if (Instance._subPanel)
        {
            Instance._subPanel.SetActive(false);
            Instance._subPanel = null;
        }
    }

    private static void AlignSubPanelAndUpdateCamera(bool isTrainInPlatform)
    {
        Instance._subPanel.transform.SetParent(Instance.transform);
        Instance._subPanel.transform.localPosition = new Vector3(0, 0, 0);
        Instance._subPanel.transform.localScale = new Vector3(1, 1, 1);
        UpdateCamera(isTrainInPlatform);
    }

    private static void UpdateCamera(bool isTrainInPlatform)
    {
        CameraManager.RightPanelActivateCameraUpdate(Instance._rightPanelWidthRatio, isTrainInPlatform);
    }


    ////////////////////////////////////////////////////
    // BACKEND FUNCTIONS
    ////////////////////////////////////////////////////

    private static List<GameObject> GetPlatformListByUnlockStatus()
    {
        List<GameObject> collection = new();
        collection.AddRange(GameObject.FindGameObjectsWithTag("PlatformLR"));
        collection.AddRange(GameObject.FindGameObjectsWithTag("PlatformTD"));
        return collection.OrderByDescending(platform => platform.GetComponent<PlatformController>().IsPlatformUnlocked)
                         .ToList();
    }

    private static void SetupSubPanel(RightPanelType rightPanelType)
    {
        Instance._subPanel.SetActive(true);
        Instance._activeRightPanelType = rightPanelType;
    }

    private static void SetupCargoPanel(GameObject train, GameObject platform, CargoTabOptions cargoTabOptions)
    {
        SetupSubPanel(RightPanelType.Cargo);

        CargoPanelManager cargoPanelMgr = Instance._subPanel.GetComponent<CargoPanelManager>();
        cargoPanelMgr.SetupCargoPanel(train, platform);

        bool trainInPlatform = train != null && platform != null;
        UpdateCamera(trainInPlatform);
        cargoPanelMgr.PopulateCargoPanel(cargoTabOptions);
    }

    ////////////////////////////////////////////////////
    // PUBLIC FUNCTIONS
    ////////////////////////////////////////////////////

    public static bool IsActivePanelSamePanelType(RightPanelType rightPanelType)
    {
        return Instance._activeRightPanelType == rightPanelType;
    }

    public static bool IsActiveCargoPanelSameTrainOrPlatform(GameObject train, GameObject platform)
    {
        if (Instance._activeRightPanelType != RightPanelType.Cargo)
            return false;

        CargoPanelManager cargoPanelMgr = Instance._subPanel.GetComponent<CargoPanelManager>();
        return cargoPanelMgr.IsSameTrainOrPlatform(train, platform);
    }

    public static CargoPanelManager GetCargoPanelManager()
    {
        if (Instance._activeRightPanelType != RightPanelType.Cargo || !Instance._subPanel)
            return default;
        return Instance._subPanel.GetComponent<CargoPanelManager>();
    }

    // Loads the cargo panel, Main entrypoint that determines what gets rendered
    public static bool LoadCargoPanel(GameObject train, GameObject platform, CargoTabOptions cargoTabOptions)
    {
        DeactivateActiveSubPanel();
        ResetRightPanel();

        if (train != null && platform == null) // When the selected train is not in the platform
            Instance._subPanel = Instance._cargoTrainOnlyPanel;
        else if (train == null && platform != null) // When the selected platform has no train
            Instance._subPanel = Instance._cargoStationOnlyPanel;
        else if (train != null && platform != null)
            Instance._subPanel = Instance._cargoTrainStationPanel;
        else
        {
            Debug.LogWarning("This should never happen! At least Either the train or the station must be valid");
            return false;
        }

        SetupCargoPanel(train, platform, cargoTabOptions);
        return true;
    }

    public static void LoadTrainList()
    {
        DeactivateActiveSubPanel();
        Instance._subPanel = Instance._FullVerticalSubPanel;
        SetupSubPanel(RightPanelType.Train);
        ResetRightPanel();
        Transform container = Instance._subPanel.transform.Find("Container");

        List<GameObject> trainList = GameObject.FindGameObjectsWithTag("Train").ToList();
        foreach (GameObject trainGO in trainList)
        {
            // TODO: Display durability and fuel stats in TrainDetailButton
            // TrainManager trainManager = trainGO.GetComponent<TrainManager>();
            // Train train = _logicMgr.GetTrainClassObject(trainManager.TrainGUID);

            GameObject trainDetailButton = Instantiate(Instance._trainDetailButtonPrefab);
            trainDetailButton.transform.SetParent(container);
            trainDetailButton.GetComponent<TrainDetailButton>().SetTrainGameObject(trainGO);
        }
        AlignSubPanelAndUpdateCamera(false);
    }

    public static void LoadPlatformList()
    {
        DeactivateActiveSubPanel();
        Instance._subPanel = Instance._FullVerticalSubPanel;
        SetupSubPanel(RightPanelType.Platform);
        ResetRightPanel();
        Transform container = Instance._subPanel.transform.Find("Container");

        List<GameObject> platformList = GetPlatformListByUnlockStatus();
        for (int i = 0; i < platformList.Count; i++)
        {
            GameObject platformDetailButton = Instantiate(Instance._platformDetailButtonPrefab);
            platformDetailButton.transform.SetParent(container);
            platformDetailButton.GetComponent<PlatformDetailButton>().SetPlatformGameObject(platformList[i]);
        }
        AlignSubPanelAndUpdateCamera(false);
    }
}
