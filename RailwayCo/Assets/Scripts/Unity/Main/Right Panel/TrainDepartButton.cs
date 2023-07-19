using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrainDepartButton : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] private Button _trainDepartButton;
    private LogicManager _logicMgr;

    private GameObject _trainToDepart;
    private GameObject _currentPlatform;

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
        _trainToDepart = train;
        _currentPlatform = platform;
        ModifyDepartButton(platform);
    }

    // Modifies the depart button for the Unified Cargo Panel
    private void ModifyDepartButton(GameObject platform)
    {

        PlatformManager pm = _currentPlatform.GetComponent<PlatformManager>();

        bool leftButtonValid = platform.GetComponent<PlatformManager>().IsLeftOrUpAccessible();
        bool rightButtonValid = platform.GetComponent<PlatformManager>().IsRightOrDownAccessible();

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


        // With the introduction of a vertical platform, we will need a new way to depart
        // By default, the naming conventions used is based on a Left/Right depart.
        if (platform.tag == "PlatformLR")
        {
            if (this.name == "LeftDepartButton")
            {
                this.transform.Find("Depart text").GetComponent<Text>().text = $"Depart to Station {pm.LeftStationNumber}";
            }
            else if (this.name == "RightDepartButton")
            {
                this.transform.Find("Depart text").GetComponent<Text>().text = $"Depart to Station {pm.RightStationNumber}";
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
                this.transform.Find("Depart text").GetComponent<Text>().text = $"Depart to Station {pm.LeftStationNumber}";
                this.name = "UpDepartButton";
            }
            else if (this.name == "RightDepartButton")
            {
                this.transform.Find("Depart text").GetComponent<Text>().text = $"Depart to Station {pm.RightStationNumber}";
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
        PlatformManager pm = _currentPlatform.GetComponent<PlatformManager>();
        TrainManager tm = _trainToDepart.GetComponent<TrainManager>();

        switch (_trainDepartButton.name)
        {
            case "LeftDepartButton":
                _logicMgr.SetStationAsDestination(tm.TrainGUID, pm.CurrentStationNumber, pm.LeftStationNumber);
                _trainToDepart.GetComponent<TrainMovement>().DepartTrain(DepartDirection.West);
                break;
            case "RightDepartButton":
                _logicMgr.SetStationAsDestination(tm.TrainGUID, pm.CurrentStationNumber, pm.RightStationNumber);
                _trainToDepart.GetComponent<TrainMovement>().DepartTrain(DepartDirection.East);
                break;
            case "UpDepartButton":
                _logicMgr.SetStationAsDestination(tm.TrainGUID, pm.CurrentStationNumber, pm.LeftStationNumber);
                _trainToDepart.GetComponent<TrainMovement>().DepartTrain(DepartDirection.North);
                break;
            case "DownDepartButton":
                _logicMgr.SetStationAsDestination(tm.TrainGUID, pm.CurrentStationNumber, pm.RightStationNumber);
                _trainToDepart.GetComponent<TrainMovement>().DepartTrain(DepartDirection.South);
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