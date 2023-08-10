using UnityEngine;
using UnityEngine.UI;

public class TrainsButton : MonoBehaviour
{
    [SerializeField] private Button _trainsButton;

    private void Awake()
    {
        if (!_trainsButton) Debug.LogError($"trainsButton not attched to {name}");
        _trainsButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked() => RightPanelManager.LoadTrainList();
}
