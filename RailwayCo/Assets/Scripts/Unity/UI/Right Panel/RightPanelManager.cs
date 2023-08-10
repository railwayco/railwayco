using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RightPanelManager : MonoBehaviour
{
    private static RightPanelManager Instance { get; set; }
    public static bool IsActiveAndEnabled => Instance != null && Instance.isActiveAndEnabled;

    [SerializeField] private CargoPanelManager _cargoPanelMgr;
    [SerializeField] private GameObject _fullVerticalSubPanel;
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

        if (!Instance._fullVerticalSubPanel) Debug.LogError("Full Subpanel not found");
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

        CameraManager.RightPanelDisableCameraUpdate();
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
        if (Instance._activeRightPanelType == RightPanelType.Cargo)
            Instance._cargoPanelMgr.DeactivateActiveCargoPanel();
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
        CameraManager.RightPanelEnableCameraUpdate(Instance._rightPanelWidthRatio, isTrainInPlatform);
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
        if (rightPanelType != RightPanelType.Cargo)
            Instance._subPanel.SetActive(true);
        Instance._activeRightPanelType = rightPanelType;
    }

    private static void SetupCargoPanel(Guid trainGuid, Guid platformGuid, CargoTabOptions cargoTabOptions)
    {
        SetupSubPanel(RightPanelType.Cargo);

        Instance._cargoPanelMgr.SetupCargoPanel(trainGuid, platformGuid);
        Instance._cargoPanelMgr.PopulateCargoPanel(cargoTabOptions);

        bool isTrainInPlatform = trainGuid != default && platformGuid != default;
        UpdateCamera(isTrainInPlatform);
    }

    ////////////////////////////////////////////////////
    // PUBLIC FUNCTIONS
    ////////////////////////////////////////////////////

    public static bool IsActivePanelSamePanelType(RightPanelType rightPanelType)
    {
        return Instance._activeRightPanelType == rightPanelType;
    }

    public static bool IsActiveCargoPanelSameTrainOrPlatform(Guid trainGuid, Guid platformGuid)
    {
        if (Instance._activeRightPanelType != RightPanelType.Cargo)
            return false;
        return Instance._cargoPanelMgr.IsSameTrainOrPlatform(trainGuid, platformGuid);
    }

    public static CargoPanelManager GetCargoPanelManager() => Instance._cargoPanelMgr;

    // Loads the cargo panel, Main entrypoint that determines what gets rendered
    public static bool LoadCargoPanel(Guid trainGuid, Guid platformGuid, CargoTabOptions cargoTabOptions)
    {
        DeactivateActiveSubPanel();
        ResetRightPanel();
        SetupCargoPanel(trainGuid, platformGuid, cargoTabOptions);
        return true;
    }

    public static void LoadTrainList()
    {
        DeactivateActiveSubPanel();
        Instance._subPanel = Instance._fullVerticalSubPanel;
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
        Instance._subPanel = Instance._fullVerticalSubPanel;
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
