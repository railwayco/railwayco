using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StationButton : MonoBehaviour
{
    [SerializeField] private Button _uiStationsButton;
    [SerializeField] private GameObject _rightPanel;

    private void Awake()
    {
        if (!_rightPanel) Debug.LogError($"Right Panel not attached to {this.name}");
        if (!_uiStationsButton) Debug.LogError($"uiTrainsButton not attched to {this.name}");
        _uiStationsButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        _rightPanel.GetComponent<RightPanelManager>().LoadStationList();
        return;
    }
}
