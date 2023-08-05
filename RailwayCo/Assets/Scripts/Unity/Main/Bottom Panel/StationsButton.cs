using UnityEngine;
using UnityEngine.UI;

public class StationsButton : MonoBehaviour
{
    [SerializeField] private Button _stationsButton;
    [SerializeField] private GameObject _rightPanel;

    private void Awake()
    {
        if (!_rightPanel) Debug.LogError($"Right Panel not attached to {this.name}");
        if (!_stationsButton) Debug.LogError($"uiTrainsButton not attched to {this.name}");
        _stationsButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        // TODO: Change from Platform List to Station with each of the platform as status (future enhancement)
        _rightPanel.GetComponent<RightPanelManager>().LoadPlatformList();
        return;
    }
}
