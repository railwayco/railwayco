using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrainDepartButton : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] private Button _trainDepartButton;

    private GameObject _trainToDepart;
    //private Guid _trainGuid;
    //private Guid _currStnGuid;
    //private Guid _destStnGuid;


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
        
        _trainToDepart = train;
        ModifyDepartButton(platform);
        //_trainGuid = train.GetComponent<TrainManager>().TrainGUID;
        //_currStnGuid = station.GetComponent<StationManager>().StationGUID;

        //Guid neighbourStationGuid;
        //switch (_trainDepartButton.name)
        //{
        //    case "LeftDepartButton":
        //        //neighbourStationGuid = _logicMgr.FindImmediateStationNeighbour(_currStnGuid, true);
        //        _departRight = false;
        //        break;
        //    case "RightDepartButton":
        //        //neighbourStationGuid = _logicMgr.FindImmediateStationNeighbour(_currStnGuid, false);
        //        _departRight = true;
        //        break;
        //    default:
        //        Debug.LogError("Not possible");
        //        //neighbourStationGuid = Guid.Empty;
        //        break;
        //}

        //_destStnGuid = neighbourStationGuid;

        //if (neighbourStationGuid == Guid.Empty)
        //{
        //    _trainDepartButton.GetComponentInChildren<Text>().text = "Depart";
        //    _trainDepartButton.GetComponent<Button>().enabled = false;
        //    _trainDepartButton.GetComponent<Image>().color = new Color(0.556f, 0.556f, 0.556f); // 0x8E8E8E
        //}
        //else
        //{
        //    string neighbourName = _logicMgr.GetIndividualStation(neighbourStationGuid).Name;
        //    _trainDepartButton.GetComponentInChildren<Text>().text = "Depart to " + neighbourName;
        //}

    }

    // Modifies the depart button for the Unified Cargo Panel
    private void ModifyDepartButton(GameObject platform)
    {

        bool leftButtonValid = platform.GetComponent<PlatformManager>().IsLeftOrUpAccessible();
        bool rightButtonValid = platform.GetComponent<PlatformManager>().IsRightOrDownAccessible();

        // Disables button if either the track or the station is unreachable
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


        // With the introduction of a vertical station, we will need a new way to depart
        // By default, the naming conventions used is based on a Left/Right depart.
        if (platform.tag == "PlatformLR")
        {
            if (this.name == "LeftDepartButton")
            {
                this.transform.Find("Depart text").GetComponent<Text>().text = "Depart Left";
            }
            else if (this.name == "RightDepartButton")
            {
                this.transform.Find("Depart text").GetComponent<Text>().text = "Depart Right";
            }
            else
            {
                Debug.LogWarning("Unknown Button name");
            }

        }
        else if (platform.tag == "PlatformTD")
        {
            if (this.name == "LeftDepartButton")
            {
                this.transform.Find("Depart text").GetComponent<Text>().text = "Depart Up";
                this.name = "UpDepartButton";
            }
            else if (this.name == "RightDepartButton")
            {
                this.transform.Find("Depart text").GetComponent<Text>().text = "Depart Down";
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
        //if (_destStnGuid == Guid.Empty) return;
        //TrainDepartStatus trainDepartStatus = _logicMgr.SetStationAsDestination(_trainGuid,
        //                                                                        _currStnGuid,
        //                                                                        _destStnGuid);
        //string eventType = "";
        //switch (trainDepartStatus)
        //{
        //    case TrainDepartStatus.OutOfFuel:
        //        eventType = "Out of fuel";
        //        break;
        //    case TrainDepartStatus.OutOfDurability:
        //        eventType = "Out of Durability";
        //        break;
        //    case TrainDepartStatus.Error:
        //        eventType = "No source station set";
        //        break;
        //    case TrainDepartStatus.Success:
        //        break;
        //}
        //if (trainDepartStatus != TrainDepartStatus.Success)
        //{
        //    TooltipManager.Show(eventType, "Error");
        //    return;
        //}

        switch (_trainDepartButton.name)
        {
            case "LeftDepartButton":
                _trainToDepart.GetComponent<TrainMovement>().DepartTrain(TrainDirection.WEST);
                break;
            case "RightDepartButton":
                _trainToDepart.GetComponent<TrainMovement>().DepartTrain(TrainDirection.EAST);
                break;
            case "UpDepartButton":
                _trainToDepart.GetComponent<TrainMovement>().DepartTrain(TrainDirection.NORTH);
                break;
            case "DownDepartButton":
                _trainToDepart.GetComponent<TrainMovement>().DepartTrain(TrainDirection.SOUTH);
                break;
            default:
                Debug.LogError("Unknown Train Depart Button Name");
                break;
        }

        _trainToDepart.GetComponent<TrainManager>().FollowTrain();
        GameObject rightPanel = GameObject.Find("MainUI").transform.Find("RightPanel").gameObject;
        rightPanel.GetComponent<RightPanelManager>().CloseRightPanel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Hide();
    }
}