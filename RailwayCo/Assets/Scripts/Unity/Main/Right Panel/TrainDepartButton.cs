using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrainDepartButton : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] private Button _trainDepartButton;
    private LogicManager _logicMgr;

    private TrainManager _trainMgr;
    private TrainMovement _trainMovement;
    private int _srcStationNum;
    private int _destStationNum;
    private DepartDirection _departDirection;
    private int _departCost;

    ////////////////////////////////////////
    /// INITIALISATION
    ////////////////////////////////////////

    private void Awake()
    {
        if (!_trainDepartButton) Debug.LogError("Train Depart Button not attached");
        _trainDepartButton.onClick.AddListener(OnButtonClicked);
        _logicMgr = GameObject.FindGameObjectsWithTag("Logic")[0].GetComponent<LogicManager>();
    }


    ////////////////////////////////////////
    /// EVENT UPDATES
    ////////////////////////////////////////
    public void SetTrainDepartInformation(GameObject train, GameObject platform)
    {
        _trainMgr = train.GetComponent<TrainManager>();
        _trainMovement = train.GetComponent<TrainMovement>();

        PlatformManager platformMgr = platform.GetComponent<PlatformManager>();
        _srcStationNum = platformMgr.CurrentStationNumber;

        ModifyDepartButton(platform);
    }

    // Modifies the depart button for the Unified Cargo Panel
    private void ModifyDepartButton(GameObject platform)
    {
        PlatformManager platformMgr = platform.GetComponent<PlatformManager>();

        bool leftButtonValid = platformMgr.IsLeftOrUpAccessible();
        bool rightButtonValid = platformMgr.IsRightOrDownAccessible();

        // Disables button if either the track or the platform is unreachable
        if (this.name == "LeftDepartButton" && !leftButtonValid)
        {
            this.GetComponent<Button>().enabled = false;
            this.GetComponent<Image>().color = new Color(0.556f, 0.556f, 0.556f); // 0x8E8E8E
        }

        if (this.name == "RightDepartButton" && !rightButtonValid)
        {
            this.GetComponent<Button>().enabled = false;
            this.GetComponent<Image>().color = new Color(0.556f, 0.556f, 0.556f); // 0x8E8E8E
        }

        int leftPathCost = platformMgr.GetLeftPathCost();
        int rightPathCost = platformMgr.GetRightPathCost();
        int leftStationNum = platformMgr.LeftStationNumber;
        int rightStationNum = platformMgr.RightStationNumber;

        // With the introduction of a vertical platform, we will need a new way to depart
        // By default, the naming conventions used is based on a Left/Right depart.
        if (platform.CompareTag("PlatformLR") && this.name == "LeftDepartButton")
        {
            _destStationNum = leftStationNum;
            _departDirection = DepartDirection.West;
            _departCost = leftPathCost;
        }
        else if (platform.CompareTag("PlatformLR") && this.name == "RightDepartButton")
        {
            _destStationNum = rightStationNum;
            _departDirection = DepartDirection.East;
            _departCost = rightPathCost;
        }
        else if (platform.CompareTag("PlatformTD") && this.name == "LeftDepartButton")
        {
            _destStationNum = leftStationNum;
            _departDirection = DepartDirection.North;
            _departCost = leftPathCost;
        }
        else if (platform.CompareTag("PlatformTD") && this.name == "RightDepartButton")
        {
            _destStationNum = rightStationNum;
            _departDirection = DepartDirection.South;
            _departCost = rightPathCost;
        }
        else if (!platform.CompareTag("PlatformLR") && !platform.CompareTag("PlatformTD"))
        {
            Debug.LogWarning("Unknown Platform tag");
            return;
        }
        else if (this.name != "LeftDepartButton" && this.name != "RightDepartButton")
        {
            Debug.LogWarning("Unknown Button name");
            return;
        }

        string destinationString = _destStationNum == 0 ? "No Station" : _destStationNum.ToString();
        string costString = _departCost.ToString();
        this.transform.Find("Depart text").GetComponent<Text>().text = destinationString;
        this.transform.Find("Cost text").GetComponent<TMP_Text>().text += costString;
    }



    ////////////////////////////////////////
    /// EVENT TRIGGERS
    ////////////////////////////////////////

    private void OnButtonClicked()
    {
        Guid trainGuid = _trainMgr.TrainGUID;

        _logicMgr.SetStationAsDestination(trainGuid, _srcStationNum, _destStationNum, _departCost);
        _trainMovement.DepartTrain(_departDirection);

        _trainMgr.FollowTrain();
        GameObject rightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        rightPanel.GetComponent<RightPanelManager>().CloseRightPanel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Hide();
    }
}