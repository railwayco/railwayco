using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrainDepartButton : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] private Button _trainDepartButton;
    private LogicManager _logicMgr;

    private TrainController _trainCtr;
    private TrainMovement _trainMovement;
    private int _srcStationNum;
    private int _destStationNum;
    private MovementDirection _departDirection;
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
        _trainCtr = train.GetComponent<TrainController>();
        _trainMovement = train.GetComponent<TrainMovement>();

        PlatformController platformCtr = platform.GetComponent<PlatformController>();
        _srcStationNum = platformCtr.CurrentStationNumber;

        ModifyDepartButton(platform);
    }

    // Modifies the depart button for the Unified Cargo Panel
    private void ModifyDepartButton(GameObject platform)
    {
        PlatformController platformCtr = platform.GetComponent<PlatformController>();

        bool leftButtonValid = platformCtr.IsLeftOrUpAccessible();
        bool rightButtonValid = platformCtr.IsRightOrDownAccessible();

        // Disables button if either the track or the platform is unreachable
        if (name == "LeftDepartButton" && !leftButtonValid)
        {
            GetComponent<Button>().enabled = false;
            GetComponent<Image>().color = new Color(0.556f, 0.556f, 0.556f); // 0x8E8E8E
        }

        if (name == "RightDepartButton" && !rightButtonValid)
        {
            GetComponent<Button>().enabled = false;
            GetComponent<Image>().color = new Color(0.556f, 0.556f, 0.556f); // 0x8E8E8E
        }

        int leftPathCost = platformCtr.GetLeftPathCost();
        int rightPathCost = platformCtr.GetRightPathCost();
        int leftStationNum = platformCtr.LeftStationNumber;
        int rightStationNum = platformCtr.RightStationNumber;

        // With the introduction of a vertical platform, we will need a new way to depart
        // By default, the naming conventions used is based on a Left/Right depart.
        if (platform.CompareTag("PlatformLR") && name == "LeftDepartButton")
        {
            _destStationNum = leftStationNum;
            _departDirection = MovementDirection.West;
            _departCost = leftPathCost;
        }
        else if (platform.CompareTag("PlatformLR") && name == "RightDepartButton")
        {
            _destStationNum = rightStationNum;
            _departDirection = MovementDirection.East;
            _departCost = rightPathCost;
        }
        else if (platform.CompareTag("PlatformTD") && name == "LeftDepartButton")
        {
            _destStationNum = leftStationNum;
            _departDirection = MovementDirection.North;
            _departCost = leftPathCost;
        }
        else if (platform.CompareTag("PlatformTD") && name == "RightDepartButton")
        {
            _destStationNum = rightStationNum;
            _departDirection = MovementDirection.South;
            _departCost = rightPathCost;
        }
        else if (!platform.CompareTag("PlatformLR") && !platform.CompareTag("PlatformTD"))
        {
            Debug.LogWarning("Unknown Platform tag");
            return;
        }
        else if (name != "LeftDepartButton" && name != "RightDepartButton")
        {
            Debug.LogWarning("Unknown Button name");
            return;
        }

        string destinationString = _destStationNum == 0 ? "No Station" : $"Station {_destStationNum}";
        string costString = _departCost.ToString();
        transform.Find("Depart text").GetComponent<Text>().text = destinationString;
        transform.Find("Cost text").GetComponent<TMP_Text>().text = $"Cost: {costString} Fuel";
    }



    ////////////////////////////////////////
    /// EVENT TRIGGERS
    ////////////////////////////////////////

    private void OnButtonClicked()
    {
        Guid trainGuid = _trainCtr.TrainGuid;

        _logicMgr.SetStationAsDestination(trainGuid, _srcStationNum, _destStationNum, _departCost);
        _trainMovement.DepartTrain(_departDirection);

        _trainCtr.FollowTrain();
        RightPanelManager.CloseRightPanel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Hide();
    }
}