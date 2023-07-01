using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TrainButton : MonoBehaviour
{
    [SerializeField] private Button _uiTrainsButton;
    [SerializeField] private GameObject _rightPanel;

    private void Awake()
    {
        if (!_rightPanel) Debug.LogError($"Right Panel not attached to {this.name}");
        if (!_uiTrainsButton) Debug.LogError($"uiTrainsButton not attched to {this.name}");
        _uiTrainsButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        _rightPanel.GetComponent<RightPanelManager>().LoadTrainList();
    }
}
