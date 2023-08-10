using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrainDepartButton : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] private Button _trainDepartButton;

    private Guid _trainGuid;
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
    }

    ////////////////////////////////////////
    /// EVENT UPDATES
    ////////////////////////////////////////
    public void SetTrainDepartInformation(GameObject train, GameObject platform)
    {
        _trainGuid = train.GetComponent<TrainController>().TrainGuid;
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
        if (name == "LeftDepartButton")
        {
            GetComponent<Button>().enabled = leftButtonValid;
            if (!leftButtonValid)
                GetComponent<Image>().color = new Color(0.556f, 0.556f, 0.556f); // 0x8E8E8E
            else
                GetComponent<Image>().color = new Color(1f, 0.756f, 0.117f); // 0xFFC11E
        }

        if (name == "RightDepartButton")
        {
            GetComponent<Button>().enabled = rightButtonValid;
            if (!rightButtonValid)
                GetComponent<Image>().color = new Color(0.556f, 0.556f, 0.556f); // 0x8E8E8E
            else
                GetComponent<Image>().color = new Color(1f, 0.756f, 0.117f); // 0xFFC11E
        }

        int leftPathCost = platformCtr.LeftPathCost;
        int rightPathCost = platformCtr.RightPathCost;
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
        TrainManager.OnTrainDeparture(_trainGuid, _srcStationNum, _destStationNum, _departCost);
        _trainMovement.DepartTrain(_departDirection);

        CameraManager.WorldCamFollowTrain(_trainGuid);
        RightPanelManager.CloseRightPanel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Hide();
    }
}