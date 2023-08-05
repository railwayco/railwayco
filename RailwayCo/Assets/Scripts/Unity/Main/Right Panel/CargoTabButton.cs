using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the behaviour of the 3 cargo Tab Buttons under the CargoTrainStationPanel
/// </summary>
public class CargoTabButton : MonoBehaviour
{
    [SerializeField] private Button _cargoButton;
    [SerializeField] private Slider _capacitySlider;
    [SerializeField] private CargoPanelManager _cargoPanelMgr;

    private Image _capacitySliderBackground;

    private void Awake()
    {
        if (!_cargoButton) Debug.LogError($"Cargo Button is not attached to {name}");
        _cargoButton.onClick.AddListener(OnButtonClicked);

        if (!_capacitySlider) Debug.LogError($"Capacity Slider is not attached to {name}");
        _capacitySliderBackground = _capacitySlider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
    }

    private void Start()
    {
        UpdateCapacity();
    }

    ////////////////////////////////////////////////////
    // EVENT FUNCTIONS
    ////////////////////////////////////////////////////

    private void OnButtonClicked()
    {
        CargoTabOptions cargoTabOptions = _cargoButton.name switch
        {
            "StationCargoButton" => CargoTabOptions.StationCargo,
            "TrainCargoButton" => CargoTabOptions.TrainCargo,
            "YardCargoButton" => CargoTabOptions.YardCargo,
            _ => CargoTabOptions.Nil
        };

        if (cargoTabOptions == CargoTabOptions.Nil)
        {
            Debug.LogError("Invalid Cargo Button Name");
            return;
        }
        _cargoPanelMgr.PopulateCargoPanel(cargoTabOptions);
    }

    public void OnHoverEnter()
    {
        Tuple<int, int, float> capacity = GetNewCapacityValue();
        int current = capacity.Item1;
        int total = capacity.Item2;
        TooltipManager.Show($"{current} / {total}", "Capacity");
    }

    public void OnHoverExit()
    {
        TooltipManager.Hide();
    }

    ////////////////////////////////////////////////////
    // CAPACITY FUNCTIONS
    ////////////////////////////////////////////////////

    private Tuple<int, int, float> GetNewCapacityValue()
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
        return new(current, total, current / (float)total);
    }

    public void UpdateCapacity()
    {
        float newValue = GetNewCapacityValue().Item3;
        _capacitySlider.value = newValue;
        _capacitySliderBackground.color = SliderGradient.GetColorIncremental(newValue);
    }
}
