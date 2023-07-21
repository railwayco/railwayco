using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PlatformTrainAddition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button _trainAdditionButton;
    [SerializeField] private GameObject _trainPrefab;

    private LogicManager _logicMgr;
    private GameObject _trainList;
    private GameObject _platform;

    private int _coinCost = 250;
    private int _specialCrateCost = 25;

    private void Awake()
    {
        if (!_trainAdditionButton) Debug.LogError("Train Addition Button not attached");
        _trainAdditionButton.onClick.AddListener(OnButtonClicked);
        _logicMgr = GameObject.FindGameObjectsWithTag("Logic")[0].GetComponent<LogicManager>();
        _trainList = GameObject.Find("TrainList");
    }

    public void UpdatePlatformInfo(GameObject platform)
    {
        _platform = platform;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Show($"Cost: {_coinCost} coins and {_specialCrateCost} purple boxes", "Add new train");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Hide();
    }

    private void OnButtonClicked()
    {
        // Perform checks
        CurrencyManager costToUnlock = new();
        costToUnlock.AddCurrency(CurrencyType.Coin, _coinCost);
        costToUnlock.AddCurrency(CurrencyType.SpecialCrate, _specialCrateCost);

        if (!_logicMgr.AbleToPurchase(costToUnlock)) return;

        // Approve sequence
        // Just close the Right Panel for now.
        GameObject rightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        rightPanel.GetComponent<RightPanelManager>().CloseRightPanel();
        TooltipManager.Hide();
        DeployNewTrain();
    }

    private void DeployNewTrain()
    {
        // The -1 for the z is needed since it is a displacement from the platform's z=0 position (standardisation)
        Vector3 deltaVertical = new Vector3(0, -0.53f, -1);
        Vector3 deltaHorizontal = new Vector3(-0.53f, 0, -1);

        if (!_platform) Debug.LogError("There is no platform to work with to deploy new train!");
        Vector3 platformPos = _platform.transform.position;

        Vector3 position = platformPos;
        Quaternion rotation = Quaternion.identity;

        if (_platform.tag == "PlatformLR")
        {
            position += deltaVertical;
        }
        else if (_platform.tag == "PlatformTD")
        {
            rotation = Quaternion.Euler(0, 0, -90);
            position += deltaHorizontal;
        }
        else
        {
            Debug.LogError($"Unknown platform tag {_platform.tag} for platform {_platform}");
        }

        TrainType trainType = TrainType.Steam;
        string platformName = _platform.name;
        Tuple<int, int> platformNums = LogicManager.GetStationPlatformNumbers(platformName);
        Guid stationGuid = _logicMgr.GetStationGuidFromStationNum(platformNums.Item1);
        Guid trainGuid = _logicMgr.AddTrainToBackend(trainType, position, rotation, stationGuid);
        _logicMgr.InitNewTrainInScene(trainGuid);
    }
}
