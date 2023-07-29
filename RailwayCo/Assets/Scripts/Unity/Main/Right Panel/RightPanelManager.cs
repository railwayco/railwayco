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
        _rightPanelWidthRatio = this.GetComponent<RectTransform>().rect.width / refReso[0];

        if (!_cargoTrainStationPanelPrefab) Debug.LogError("Train Station Yard Cargo Panel Prefab not found");
        if (!_cargoStationOnlyPanelPrefab) Debug.LogError("Station Yard Cargo Panel Prefab not found");
        if (!_cargoTrainOnlyPanelPrefab) Debug.LogError("Train Yard Cargo Panel Prefab not found");
        if (!_FullVerticalSubPanelPrefab) Debug.LogError("Full Subpanel Prefab not found");
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

        if (_activeRightPanelType == RightPanelType.Cargo)
        {
            Destroy(_subPanel);
            _subPanel = null;
        }
    }

    private void ResetRightPanel()
    {
        if (!this.gameObject.activeInHierarchy) this.gameObject.SetActive(true);
        DestroyRightPanelChildren();
        if (_activeRightPanelType == RightPanelType.Cargo && _subPanel)
        {
            Destroy(_subPanel);
        }
        _subPanel = null;
    }

    private void DestroyRightPanelChildren()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void AlignSubPanelAndUpdateCamera(bool isTrainInPlatform)
    {
        _subPanel.transform.SetParent(this.transform);
        _subPanel.transform.localPosition = new Vector3(0, 0, 0);
        _subPanel.transform.localScale = new Vector3(1, 1, 1);
        _camMgr.RightPanelActivateCameraUpdate(_rightPanelWidthRatio, isTrainInPlatform);
    }


    ////////////////////////////////////////////////////
    // BACKEND FUNCTIONS
    ////////////////////////////////////////////////////

    private List<GameObject> GetPlatformListByUnlockStatus()
    {
        List<GameObject> unlockedPlatformList = new();
        List<GameObject> lockedPlatformList = new();

        List<GameObject[]> tmp = new();
        tmp.Add(GameObject.FindGameObjectsWithTag("PlatformLR"));
        tmp.Add(GameObject.FindGameObjectsWithTag("PlatformTD"));

        foreach (GameObject[] collection in tmp)
        {
            foreach (GameObject platform in collection)
            {
                if (platform.GetComponent<PlatformManager>().IsPlatformUnlocked)
                {
                    unlockedPlatformList.Add(platform);
                }
                else
                {
                    lockedPlatformList.Add(platform);
                }
            }
        }

        unlockedPlatformList.AddRange(lockedPlatformList);
        return unlockedPlatformList;
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

    // Loads the cargo panel, Main entrypoint that determines what gets rendered
    public bool LoadCargoPanel(GameObject train, GameObject platform, CargoTabOptions cargoTabOptions)
    {
        ResetRightPanel();

        GameObject cargoPrefab;
        if (train != null && platform == null) // When the selected train is not in the platform
            cargoPrefab = _cargoTrainOnlyPanelPrefab;
        else if (train == null && platform != null) // When the selected platform has no train
            cargoPrefab = _cargoStationOnlyPanelPrefab;
        else if (train != null && platform != null)
            cargoPrefab = _cargoTrainStationPanelPrefab;
        else
        {
            Debug.LogWarning("This should never happen! At least Either the train or the station must be valid");
            return false;
        }

        _subPanel = CargoPanelManager.Init(cargoPrefab, train, platform);
        if (!_subPanel)
        {
            ResetRightPanel();
            return false;
        }
        _activeRightPanelType = RightPanelType.Cargo;

        bool trainInPlatform = train != null && platform != null;
        AlignSubPanelAndUpdateCamera(trainInPlatform);
        CargoPanelManager cargoPanelMgr = _subPanel.GetComponent<CargoPanelManager>();
        cargoPanelMgr.PopulateCargoPanel(cargoTabOptions);
        return true;
    }

    public void LoadTrainList()
    {
        ResetRightPanel();

        _subPanel = Instantiate(_FullVerticalSubPanelPrefab);
        _activeRightPanelType = RightPanelType.Train;
        Transform container = _subPanel.transform.Find("Container");

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
        AlignSubPanelAndUpdateCamera(false);
    }

    public void LoadPlatformList()
    {
        ResetRightPanel();

        _subPanel = Instantiate(_FullVerticalSubPanelPrefab);
        _activeRightPanelType = RightPanelType.Platform;
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
