using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the behaviour of the 3 cargo Tab Buttons under the CargoTrainStationPanel
/// </summary>
public class CargoTabButton : MonoBehaviour
{
    [SerializeField] private Button _cargoButton;
    private CargoPanelManager _cargoPanelMgr;

    private void Awake()
    {
        if (!_cargoButton) Debug.LogError($"Cargo Button is not attached to {this.name}");
        _cargoButton.onClick.AddListener(OnButtonClicked);

    }

    private void Start()
    {
        GameObject rightPanel = GameObject.FindGameObjectWithTag("MainUI").transform.Find("RightPanel").gameObject;
        _cargoPanelMgr = rightPanel.GetComponentInChildren<CargoPanelManager>(true);
        if (!_cargoPanelMgr) Debug.LogError("CargoPanelManager not found");
    }

    private void OnButtonClicked()
    {
        CargoTabOptions cargoTabOptions;

        if (_cargoButton.name == "StationCargoButton")
        {
            cargoTabOptions = CargoTabOptions.StationCargo;
        }
        else if (_cargoButton.name == "TrainCargoButton")
        {
            cargoTabOptions = CargoTabOptions.TrainCargo;
        }
        else if (_cargoButton.name == "YardCargoButton")
        {
            cargoTabOptions = CargoTabOptions.YardCargo;
        }
        else
        {
            Debug.LogWarning("Invalid Cargo Button Name");
            return;
        }
        _cargoPanelMgr.PopulateCargoPanel(cargoTabOptions);
    }
}
