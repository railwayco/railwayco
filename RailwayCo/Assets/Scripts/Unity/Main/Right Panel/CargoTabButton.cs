using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the behaviour of the 3 cargo Tab Buttons under the CargoTrainStationPanel
/// </summary>
public class CargoTabButton : MonoBehaviour
{
    [SerializeField] private Button _cargoButton;
    private RightPanelManager _rightPanelMgrScript;

    // Depending on the cargoButton that this script is associated with, either one will be set to Guid.Empty by the RightPanel manager when
    private GameObject _platform;
    private GameObject _train;

    public void SetCargoTabButtonInformation(GameObject trainObject, GameObject platformObject)
    {
        _train = trainObject;
        _platform= platformObject;
    }

    private void Awake()
    {
        if (!_cargoButton) Debug.LogError($"Cargo Button is not attached to {this.name}");
        _cargoButton.onClick.AddListener(OnButtonClicked);

        GameObject RightPanel = GameObject.FindGameObjectWithTag("MainUI").transform.Find("RightPanel").gameObject;
        _rightPanelMgrScript = RightPanel.GetComponent<RightPanelManager>();
    }

    private void OnButtonClicked()
    {
        if (_cargoButton.name == "StationCargoButton")
        {
            _rightPanelMgrScript.UpdateChosenCargoTab(RightPanelManager.CargoTabOptions.STATION_CARGO);
            
        }
        else if (_cargoButton.name == "TrainCargoButton")
        {
            _rightPanelMgrScript.UpdateChosenCargoTab(RightPanelManager.CargoTabOptions.TRAIN_CARGO);
            
        }
        else if (_cargoButton.name == "YardCargoButton")
        {
            _rightPanelMgrScript.UpdateChosenCargoTab(RightPanelManager.CargoTabOptions.YARD_CARGO);
        }
        else
        {
            Debug.LogWarning("Invalid Cargo Button Name");
            return;
        }
        _rightPanelMgrScript.LoadCargoPanel(_train, _platform);
    }
}
