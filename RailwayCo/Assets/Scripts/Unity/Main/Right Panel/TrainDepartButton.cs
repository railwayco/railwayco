using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrainDepartButton : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] private Button _trainDepartButton;
    private LogicManager _logicMgr;

    private GameObject _trainToDepart;
    private Guid _trainGuid;
    private Guid _currStnGuid;
    private Guid _destStnGuid;
    private bool _departRight;

    public void SetTrainDepartInformation(GameObject train, GameObject station)
    {
        _trainToDepart = train;
        _trainGuid = train.GetComponent<TrainManager>().TrainGUID;
        _currStnGuid = station.GetComponent<StationManager>().StationGUID;

        Guid neighbourStationGuid;
        switch (_trainDepartButton.name)
        {
            case "LeftDepartButton":
                neighbourStationGuid = _logicMgr.FindImmediateStationNeighbour(_currStnGuid, true);
                _departRight = false;
                break;
            case "RightDepartButton":
                neighbourStationGuid = _logicMgr.FindImmediateStationNeighbour(_currStnGuid, false);
                _departRight = true;
                break;
            default:
                Debug.LogError("Not possible");
                neighbourStationGuid = Guid.Empty;
                break;
        }

        _destStnGuid = neighbourStationGuid;

        if (neighbourStationGuid == Guid.Empty)
        {
            _trainDepartButton.GetComponentInChildren<Text>().text = "Depart";
            _trainDepartButton.GetComponent<Button>().enabled = false;
            _trainDepartButton.GetComponent<Image>().color = new Color(0.556f, 0.556f, 0.556f); // 0x8E8E8E
        }
        else
        {
            string neighbourName = _logicMgr.GetIndividualStation(neighbourStationGuid).Name;
            _trainDepartButton.GetComponentInChildren<Text>().text = "Depart to " + neighbourName;
        }

    }

    private void Awake()
    {
        if (!_trainDepartButton) Debug.LogError("Train Depart Button not attached");
        _trainDepartButton.onClick.AddListener(OnButtonClicked);

        _logicMgr = GameObject.Find("LogicManager").GetComponent<LogicManager>();
    }

    private void OnButtonClicked()
    {
        if (_destStnGuid == Guid.Empty) return;
        TrainDepartStatus trainDepartStatus = _logicMgr.SetStationAsDestination(_trainGuid,
                                                                                _currStnGuid,
                                                                                _destStnGuid);
        string eventType = "";
        switch (trainDepartStatus)
        {
            case TrainDepartStatus.OutOfFuel:
                eventType = "Out of fuel";
                break;
            case TrainDepartStatus.OutOfDurability:
                eventType = "Out of Durability";
                break;
            case TrainDepartStatus.Error:
                eventType = "No source station set";
                break;
            case TrainDepartStatus.Success:
                break;
        }
        if (trainDepartStatus != TrainDepartStatus.Success)
        {
            TooltipManager.Show(eventType, "Error");
            return;
        }

        _trainToDepart.GetComponent<TrainMovement>().DepartTrain(_departRight);

        GameObject rightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        rightPanel.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Hide();
    }
}