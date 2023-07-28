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
    private PlatformManager _platformMgr;
    private string _platformTag;

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

        _platformMgr = platform.GetComponent<PlatformManager>();
        _platformTag = platform.tag;

        ModifyDepartButton();
    }

    // Modifies the depart button for the Unified Cargo Panel
    private void ModifyDepartButton()
    {
        bool leftButtonValid = _platformMgr.IsLeftOrUpAccessible();
        bool rightButtonValid = _platformMgr.IsRightOrDownAccessible();

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

        int leftPathCost = _platformMgr.GetLeftPathCost();
        int rightPathCost = _platformMgr.GetRightPathCost();
        int leftStationNum = _platformMgr.LeftStationNumber;
        int rightStationNum = _platformMgr.RightStationNumber;

        // With the introduction of a vertical platform, we will need a new way to depart
        // By default, the naming conventions used is based on a Left/Right depart.
        if (_platformTag == "PlatformLR")
        {
            if (this.name == "LeftDepartButton")
            {
                this.transform.Find("Depart text").GetComponent<Text>().text += leftStationNum.ToString();
                this.transform.Find("Cost text").GetComponent<TMP_Text>().text += leftPathCost.ToString();
            }
            else if (this.name == "RightDepartButton")
            {
                this.transform.Find("Depart text").GetComponent<Text>().text += rightStationNum.ToString();
                this.transform.Find("Cost text").GetComponent<TMP_Text>().text += rightPathCost.ToString();
            }
            else
            {
                Debug.LogWarning("Unknown Button name");
            }

        }
        else if (_platformTag == "PlatformTD")
        {
            if (this.name == "LeftDepartButton")
            {
                this.transform.Find("Depart text").GetComponent<Text>().text += leftStationNum.ToString();
                this.transform.Find("Cost text").GetComponent<TMP_Text>().text += leftPathCost.ToString();
                this.name = "UpDepartButton";
            }
            else if (this.name == "RightDepartButton")
            {
                this.transform.Find("Depart text").GetComponent<Text>().text += rightStationNum.ToString();
                this.transform.Find("Cost text").GetComponent<TMP_Text>().text += rightPathCost.ToString();
                this.name = "DownDepartButton";
            }
            else
            {
                Debug.LogWarning("Unknown Button name");
            }
        }
        else
        {
            Debug.LogError("Unknown tag for a station platform");
        }
    }



    ////////////////////////////////////////
    /// EVENT TRIGGERS
    ////////////////////////////////////////

    private void OnButtonClicked()
    {
        Guid trainGuid = _trainMgr.TrainGUID;
        int sourceStationNum = _platformMgr.CurrentStationNumber;
        int leftStationNum = _platformMgr.LeftStationNumber;
        int rightStationNum = _platformMgr.RightStationNumber;

        switch (_trainDepartButton.name)
        {
            case "LeftDepartButton":
                _logicMgr.SetStationAsDestination(trainGuid, sourceStationNum, leftStationNum);
                _trainMovement.DepartTrain(DepartDirection.West);
                break;
            case "RightDepartButton":
                _logicMgr.SetStationAsDestination(trainGuid, sourceStationNum, rightStationNum);
                _trainMovement.DepartTrain(DepartDirection.East);
                break;
            case "UpDepartButton":
                _logicMgr.SetStationAsDestination(trainGuid, sourceStationNum, leftStationNum);
                _trainMovement.DepartTrain(DepartDirection.North);
                break;
            case "DownDepartButton":
                _logicMgr.SetStationAsDestination(trainGuid, sourceStationNum, rightStationNum);
                _trainMovement.DepartTrain(DepartDirection.South);
                break;
            default:
                Debug.LogError("Unknown Train Depart Button Name");
                break;
        }

        _trainMgr.FollowTrain();
        GameObject rightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        rightPanel.GetComponent<RightPanelManager>().CloseRightPanel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Hide();
    }
}