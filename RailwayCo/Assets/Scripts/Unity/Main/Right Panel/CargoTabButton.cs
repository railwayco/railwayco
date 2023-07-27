using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the behaviour of the 3 cargo Tab Buttons under the CargoTrainStationPanel
/// </summary>
public class CargoTabButton : MonoBehaviour
{
    [SerializeField] private Button _cargoButton;
    [SerializeField] private Slider _capacitySlider;
    private CargoPanelManager _cargoPanelMgr;

    private void Awake()
    {
        if (!_cargoButton) Debug.LogError($"Cargo Button is not attached to {this.name}");
        _cargoButton.onClick.AddListener(OnButtonClicked);

        if (!_capacitySlider) Debug.LogError($"Capacity Slider is not attached to {this.name}");
    }

    private void Start()
    {
        GameObject rightPanel = GameObject.FindGameObjectWithTag("MainUI").transform.Find("RightPanel").gameObject;
        _cargoPanelMgr = rightPanel.GetComponentInChildren<CargoPanelManager>(true);
        if (!_cargoPanelMgr) Debug.LogError("CargoPanelManager not found");
        UpdateCapacity();
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

    public void UpdateCapacity()
    {
        int current, total;
        if (_cargoButton.name.Contains("Train"))
        {
            IntAttribute capacity = _cargoPanelMgr.GetTrainCapacity();
            current = capacity.Amount;
            total = capacity.UpperLimit;
        }
        else if (_cargoButton.name.Contains("Yard"))
        {
            IntAttribute capacity = _cargoPanelMgr.GetYardCapacity();
            current = capacity.Amount;
            total = capacity.UpperLimit;
        }
        else
        {
            // Station Cargo button
            current = _cargoPanelMgr.GetStationCargoList().Count;
            total = 10; // Hardcoded
        }
        _capacitySlider.value = current / (float)total;
    }
}
