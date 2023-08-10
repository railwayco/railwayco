using UnityEngine;
using UnityEngine.UI;

public class StationsButton : MonoBehaviour
{
    [SerializeField] private Button _stationsButton;

    private void Awake()
    {
        if (!_stationsButton) Debug.LogError($"stationsButton not attched to {name}");
        _stationsButton.onClick.AddListener(OnButtonClicked);
    }

    // TODO: Change from Platform List to Station with each of the platform as status (future enhancement)
    private void OnButtonClicked() => RightPanelManager.LoadPlatformList();
}
