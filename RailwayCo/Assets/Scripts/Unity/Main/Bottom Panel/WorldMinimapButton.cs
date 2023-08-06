using UnityEngine;
using UnityEngine.UI;

public class WorldMinimapButton : MonoBehaviour
{
    [SerializeField] private Button _worldMinimapButton;
    
    private void Start()
    {
        if (!_worldMinimapButton) Debug.LogError("uiWorldButton not attached");
        _worldMinimapButton.onClick.AddListener(OnButtonClicked);

        CameraManager.SetDefaultWorldView(true);
    }

    private void OnButtonClicked()
    {
        Text displayText = _worldMinimapButton.transform.Find("UI_WorldMinimapText").GetComponent<Text>();
        string inactiveCamera = CameraManager.ToggleWorldMinimapCamera(displayText.text);
        switch (inactiveCamera)
        {
            case "World":
            case "Minimap":
                displayText.text = inactiveCamera;
                break;
            default:
                Debug.LogWarning("Unknown Active Camera text");
                break;
        } 
    }
}
