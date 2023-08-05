using UnityEngine;
using TMPro;

public class WelcomeScript : MonoBehaviour
{
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _formPanel;
    [SerializeField] private TMP_Text _infoTextMsg;
    [SerializeField] private SceneChanger _sceneChanger;

    private void Awake()
    {
        Screen.SetResolution(1280, 720, false);

        if (!_menuPanel) Debug.LogError("Menu Panel not attached to WelcomeScript");
        if (!_formPanel) Debug.LogError("Form Panel not attached to WelcomeScript");
        if (!_infoTextMsg) Debug.LogError("Info Text Msg not attached to WelcomeScript");
        if (!_sceneChanger) Debug.LogError("Scene Changer not attached to WelcomeScript");
    }

    private void Start()
    {
        AuthManager.SuccessHandler += AuthManager_SuccessHandler;
        AuthManager.ErrorHandler += AuthManager_ErrorHandler;
    }

    private void AuthManager_SuccessHandler(object sender, string authEvent)
    {
        _infoTextMsg.color = new Color32(0, 255, 25, 255);
        _infoTextMsg.text = $"{authEvent} successful";
        ChangeToLoadingScene();
    }

    private void AuthManager_ErrorHandler(object sender, string errorMsg)
    {
        _infoTextMsg.color = new Color32(255, 110, 0, 255);
        _infoTextMsg.text = errorMsg;
    }

    public void ResetInfoTextMsg()
    {
        // Reset infoTextMsg after a button clicked
        _infoTextMsg.color = new Color32(255, 255, 255, 255);
        _infoTextMsg.text = "";
    }

    public void SwitchToMenu()
    {
        _formPanel.SetActive(false);
        _menuPanel.SetActive(true);
        ResetInfoTextMsg();
    }

    public void SwitchToForm(bool isSignIn)
    {
        _menuPanel.SetActive(false);
        _formPanel.SetActive(true);
        _formPanel.GetComponent<UserAuth>().SetupForm(isSignIn);
        ResetInfoTextMsg();
    }

    public void ChangeToLoadingScene() => _sceneChanger.sceneChangeEvent.Invoke(Scene.Loading);
}
