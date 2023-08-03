using UnityEngine;
using TMPro;

public class WelcomeScript : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject formPanel;
    [SerializeField] private TMP_Text infoTextMsg;
    [SerializeField] private SceneChanger _sceneChanger;

    private void Awake()
    {
        Screen.SetResolution(1280, 720, false);
    }

    private void Start()
    {
        AuthManager.SuccessHandler += AuthManager_SuccessHandler;
        AuthManager.ErrorHandler += AuthManager_ErrorHandler;
    }

    private void AuthManager_SuccessHandler(object sender, string authEvent)
    {
        infoTextMsg.color = new Color32(0, 255, 25, 255);
        infoTextMsg.text = $"{authEvent} successful";
        ChangeToLoadingScene();
    }

    private void AuthManager_ErrorHandler(object sender, string errorMsg)
    {
        infoTextMsg.color = new Color32(255, 110, 0, 255);
        infoTextMsg.text = errorMsg;
    }

    public void ResetInfoTextMsg()
    {
        // Reset infoTextMsg after a button clicked
        infoTextMsg.color = new Color32(255, 255, 255, 255);
        infoTextMsg.text = "";
    }

    public void SwitchToMenu()
    {
        formPanel.SetActive(false);
        menuPanel.SetActive(true);
        ResetInfoTextMsg();
    }

    public void SwitchToForm(bool isSignIn)
    {
        menuPanel.SetActive(false);
        formPanel.SetActive(true);
        formPanel.GetComponent<UserAuth>().SetupForm(isSignIn);
        ResetInfoTextMsg();
    }

    public void ChangeToLoadingScene() => _sceneChanger.sceneChangeEvent.Invoke(Scene.Loading);
}
