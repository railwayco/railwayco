using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TrainButton : MonoBehaviour
{
    [SerializeField] private Button _uiTrainsButton;
    [SerializeField] private GameObject _rightPanel;
    [SerializeField] private GameObject _rightSubPanelPrefab;
    [SerializeField] private GameObject _trainCellPrefab;


    void Awake()
    {
        _uiTrainsButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        _rightPanel.GetComponent<RightPanelManager>().LoadTrainList();
        return;
    }
}
