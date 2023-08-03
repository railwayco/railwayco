using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button newGameBtn;
    [SerializeField] private Button contGameBtn;
    [SerializeField] private Button loginBtn;
    [SerializeField] private Button logoutBtn;
    [SerializeField] private Button createAccBtn;
    [SerializeField] private Button settingsBtn;

    [SerializeField] private WelcomeScript welcomeScript;

    void Start()
    {
        newGameBtn.onClick.AddListener(AuthManager.LoginWithCustomID);
        contGameBtn.onClick.AddListener(welcomeScript.ChangeToLoadingScene);
        loginBtn.onClick.AddListener(() => SwitchToForm(true));
        logoutBtn.onClick.AddListener(AuthManager.Logout);
        createAccBtn.onClick.AddListener(() => SwitchToForm(false));
        settingsBtn.onClick.AddListener(Settings);
    }

    void Update()
    {
        bool isLoggedIn = AuthManager.IsLoggedIn();

        newGameBtn.gameObject.SetActive(!isLoggedIn);
        loginBtn.gameObject.SetActive(!isLoggedIn);
        createAccBtn.gameObject.SetActive(!isLoggedIn);

        contGameBtn.gameObject.SetActive(isLoggedIn);
        logoutBtn.gameObject.SetActive(isLoggedIn);
        settingsBtn.gameObject.SetActive(isLoggedIn);
    }

    private void SwitchToForm(bool isSignIn) => welcomeScript.SwitchToForm(isSignIn);

    // Placeholder method
    private void Settings() { }
}
