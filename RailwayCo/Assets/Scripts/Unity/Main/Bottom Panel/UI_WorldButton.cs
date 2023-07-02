using UnityEngine;
using UnityEngine.UI;

public class UI_WorldButton : MonoBehaviour
{
    [SerializeField] private Button _uiWorldButton;
    private CameraManager _camMgr;
    
    private void Start()
    {
        if (!_uiWorldButton) Debug.LogError("uiWorldButton not attached");
        _uiWorldButton.onClick.AddListener(OnButtonClicked);

        GameObject camList = GameObject.Find("CameraList");
        if (camList == null) Debug.LogError("Unable to find Camera List");
        _camMgr = camList.GetComponent<CameraManager>();
        if (!_camMgr) Debug.LogError("There is no Camera Manager attached to the camera list!");

        _camMgr.SetDefaultWorldView(true);
    }

    private void OnButtonClicked()
    {
        _camMgr.SetDefaultWorldView(false);
    }
}
