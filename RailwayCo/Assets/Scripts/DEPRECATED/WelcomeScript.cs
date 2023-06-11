using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WelcomeScript : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject formPanel;
    [SerializeField] private Button newGameBtn;
    [SerializeField] private Button contGameBtn;
    [SerializeField] private Button loginBtn;
    [SerializeField] private Button logoutBtn;
    [SerializeField] private Button createAccBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button signInBtn;
    [SerializeField] private Button signUpBtn;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Button crossOutBtn;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField usernameInput;

    private AuthManager authManager = new(); // TODO: AuthManager needs to exist between scenes

    public AuthManager AuthManager { get => authManager; set => authManager = value; }

    void Start()
    {
        newGameBtn.onClick.AddListener(() => OnButtonClicked(ButtonType.NewGame));
        contGameBtn.onClick.AddListener(() => OnButtonClicked(ButtonType.ContGame));
        loginBtn.onClick.AddListener(() => OnButtonClicked(ButtonType.Login));
        logoutBtn.onClick.AddListener(() => OnButtonClicked(ButtonType.Logout));
        createAccBtn.onClick.AddListener(() => OnButtonClicked(ButtonType.CreateAcc));
        settingsBtn.onClick.AddListener(() => OnButtonClicked(ButtonType.Settings));

        signInBtn.onClick.AddListener(() => OnButtonClicked(ButtonType.SignIn));
        signUpBtn.onClick.AddListener(() => OnButtonClicked(ButtonType.SignUp));
        cancelBtn.onClick.AddListener(() => OnButtonClicked(ButtonType.Cancel));
        crossOutBtn.onClick.AddListener(() => OnButtonClicked(ButtonType.CrossOut));

        formPanel.SetActive(false);
        Update();
    }

    void Update()
    {
        bool isLoggedIn = AuthManager.IsLoggedIn();
        if (!isLoggedIn)
        {
            newGameBtn.gameObject.SetActive(true);
            contGameBtn.gameObject.SetActive(false);
            loginBtn.gameObject.SetActive(true);
            logoutBtn.gameObject.SetActive(false);
            createAccBtn.gameObject.SetActive(true);
            settingsBtn.gameObject.SetActive(false);
        }
        else
        {
            newGameBtn.gameObject.SetActive(false);
            contGameBtn.gameObject.SetActive(true);
            loginBtn.gameObject.SetActive(false);
            logoutBtn.gameObject.SetActive(true);
            createAccBtn.gameObject.SetActive(false);
            settingsBtn.gameObject.SetActive(true);
        }
    }

    private void OnButtonClicked(ButtonType menuButton)
    {
        Debug.Log(menuButton.ToString() + " button clicked!");

        switch (menuButton)
        {
            case ButtonType.NewGame:
                {
                    AuthManager.LoginWithCustomID();
                    new SceneTransition().OnButtonClicked(); // Temporary solution
                    break;
                }
            case ButtonType.ContGame:
                {
                    new SceneTransition().OnButtonClicked(); // Temporary solution
                    break;
                }
            case ButtonType.Login:
                {
                    SwitchToForm();
                    signInBtn.gameObject.SetActive(true);
                    signUpBtn.gameObject.SetActive(false);
                    usernameInput.gameObject.SetActive(false);
                    break;
                }
            case ButtonType.Logout:
                {
                    AuthManager.Logout();
                    Update();
                    break;
                }
            case ButtonType.CreateAcc:
                {
                    SwitchToForm();
                    signInBtn.gameObject.SetActive(false);
                    signUpBtn.gameObject.SetActive(true);
                    usernameInput.gameObject.SetActive(true);
                    break;
                }
            case ButtonType.Settings:
                {
                    // TODO: Implement settings
                    break;
                }
            case ButtonType.SignIn:
                {
                    string email = emailInput.text;
                    string password = passwordInput.text;
                    AuthManager.LoginWithEmailAddress(email, password);

                    // TODO: Check if sign in successful

                    SwitchToMenu();
                    Update();
                    break;
                }
            case ButtonType.SignUp:
                {
                    string email = emailInput.text;
                    string password = passwordInput.text;
                    string username = usernameInput.text;
                    AuthManager.RegisterUser(email, password, username);

                    // TODO: Check if sign up successful

                    SwitchToMenu();
                    break;
                }
            case ButtonType.Cancel:
                {
                    SwitchToMenu();
                    break;
                }
            case ButtonType.CrossOut:
                {
                    SwitchToMenu();
                    break;
                }
        }
    }

    private void SwitchToMenu()
    {
        emailInput.text = "";
        passwordInput.text = "";
        usernameInput.text = "";
        formPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    private void SwitchToForm()
    {
        menuPanel.SetActive(false);
        formPanel.SetActive(true);
    }
}

public enum ButtonType
{
    NewGame,
    ContGame,
    Login,
    Logout,
    CreateAcc,
    Settings,
    SignIn,
    SignUp,
    Cancel,
    CrossOut
}
