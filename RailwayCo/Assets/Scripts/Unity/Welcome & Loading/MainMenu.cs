using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _newGameBtn;
    [SerializeField] private Button _contGameBtn;
    [SerializeField] private Button _loginBtn;
    [SerializeField] private Button _logoutBtn;
    [SerializeField] private Button _createAccBtn;
    [SerializeField] private Button _settingsBtn;
    [SerializeField] private WelcomeScript _welcomeScript;

    private void Awake()
    {
        if (!_newGameBtn) Debug.LogError("New Game Button not attached to MainMenu");
        if (!_contGameBtn) Debug.LogError("Cont Game Button not attached to MainMenu");
        if (!_loginBtn) Debug.LogError("Login Button not attached to MainMenu");
        if (!_logoutBtn) Debug.LogError("Logout Button not attached to MainMenu");
        if (!_createAccBtn) Debug.LogError("Create Account Button not attached to MainMenu");
        if (!_settingsBtn) Debug.LogError("Settings Button not attached to MainMenu");
        if (!_welcomeScript) Debug.LogError("Welcome Script not attached to MainMenu");
    }

    private void Start()
    {
        _newGameBtn.onClick.AddListener(AuthManager.LoginWithCustomID);
        _contGameBtn.onClick.AddListener(_welcomeScript.ChangeToLoadingScene);
        _loginBtn.onClick.AddListener(() => SwitchToForm(true));
        _logoutBtn.onClick.AddListener(AuthManager.Logout);
        _createAccBtn.onClick.AddListener(() => SwitchToForm(false));
        _settingsBtn.onClick.AddListener(Settings);
    }

    private void Update()
    {
        bool isLoggedIn = AuthManager.IsLoggedIn();

        _newGameBtn.gameObject.SetActive(!isLoggedIn);
        _loginBtn.gameObject.SetActive(!isLoggedIn);
        _createAccBtn.gameObject.SetActive(!isLoggedIn);

        _contGameBtn.gameObject.SetActive(isLoggedIn);
        _logoutBtn.gameObject.SetActive(isLoggedIn);
        _settingsBtn.gameObject.SetActive(isLoggedIn);
    }

    private void SwitchToForm(bool isSignIn) => _welcomeScript.SwitchToForm(isSignIn);

    // Placeholder method
    private void Settings() { }
}
