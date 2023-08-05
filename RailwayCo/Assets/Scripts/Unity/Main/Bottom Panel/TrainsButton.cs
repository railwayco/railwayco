using UnityEngine;
using UnityEngine.UI;

public class TrainsButton : MonoBehaviour
{
    [SerializeField] private Button _trainsButton;
    [SerializeField] private RightPanelManager _rightPanelMgr;

    private void Awake()
    {
        if (!_rightPanelMgr) Debug.LogError($"Right Panel Manager not attached to {this.name}");
        if (!_trainsButton) Debug.LogError($"trainsButton not attched to {this.name}");
        _trainsButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked() => _rightPanelMgr.LoadTrainList();
}
