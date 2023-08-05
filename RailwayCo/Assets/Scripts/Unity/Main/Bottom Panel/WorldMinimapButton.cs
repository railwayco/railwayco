using UnityEngine;
using UnityEngine.UI;

public class WorldMinimapButton : MonoBehaviour
{
    [SerializeField] private Button _worldMinimapButton;
    private CameraManager _camMgr;
    
    private void Start()
    {
        if (!_worldMinimapButton) Debug.LogError("uiWorldButton not attached");
        _worldMinimapButton.onClick.AddListener(OnButtonClicked);

        GameObject camList = GameObject.Find("CameraList");
        if (camList == null) Debug.LogError("Unable to find Camera List");
        _camMgr = camList.GetComponent<CameraManager>();
        if (!_camMgr) Debug.LogError("There is no Camera Manager attached to the camera list!");

        _camMgr.SetDefaultWorldView(true);
    }

    private void OnButtonClicked()
    {
        Text displayText = _worldMinimapButton.transform.Find("UI_WorldMinimapText").GetComponent<Text>();
        string inactiveCamera = _camMgr.ToggleWorldMinimapCamera(displayText.text);
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
