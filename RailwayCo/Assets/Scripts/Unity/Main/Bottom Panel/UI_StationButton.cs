using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StationButton : MonoBehaviour
{
    [SerializeField] private Button _uiStationsButton;
    [SerializeField] private GameObject _rightPanel;
    [SerializeField] private GameObject _rightSubPanelPrefab;
    [SerializeField] private GameObject _stationCellPrefab;

    void Awake()
    {
        _uiStationsButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        _rightPanel.GetComponent<RightPanelManager>().LoadStationList();
        return;
    }
}
