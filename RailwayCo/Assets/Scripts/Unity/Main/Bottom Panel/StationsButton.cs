using UnityEngine;
using UnityEngine.UI;

public class StationsButton : MonoBehaviour
{
    [SerializeField] private Button _stationsButton;
    [SerializeField] private RightPanelManager _rightPanelMgr;

    private void Awake()
    {
        if (!_rightPanelMgr) Debug.LogError($"Right Panel Manager not attached to {this.name}");
        if (!_stationsButton) Debug.LogError($"stationsButton not attched to {this.name}");
        _stationsButton.onClick.AddListener(OnButtonClicked);
    }

    // TODO: Change from Platform List to Station with each of the platform as status (future enhancement)
    private void OnButtonClicked() => _rightPanelMgr.LoadPlatformList();
}
