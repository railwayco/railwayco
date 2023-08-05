using UnityEngine;
using UnityEngine.UI;

public class TrainsButton : MonoBehaviour
{
    [SerializeField] private Button _trainsButton;
    [SerializeField] private GameObject _rightPanel;

    private void Awake()
    {
        if (!_rightPanel) Debug.LogError($"Right Panel not attached to {this.name}");
        if (!_trainsButton) Debug.LogError($"uiTrainsButton not attched to {this.name}");
        _trainsButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        _rightPanel.GetComponent<RightPanelManager>().LoadTrainList();
    }
}
