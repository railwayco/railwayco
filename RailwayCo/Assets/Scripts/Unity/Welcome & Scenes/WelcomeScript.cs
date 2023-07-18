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
    [SerializeField] private TMP_Text infoTextMsg;

    [SerializeField] private GameLogic _gameLogic;
    [SerializeField] private SceneChanger _sceneChanger;

    public SceneChanger SceneChanger { get => _sceneChanger; private set => _sceneChanger = value; }
    public GameLogic GameLogic { get => _gameLogic; private set => _gameLogic = value; }

    enum ButtonType
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

    void Start()
    {
        AuthManager.SuccessHandler += AuthManager_SuccessHandler;
        AuthManager.ErrorHandler += AuthManager_ErrorHandler;

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

    private void OnButtonClicked(ButtonType menuButton)
    {
        Debug.Log(menuButton.ToString() + " button clicked!");

        // Reset infoTextMsg after a button clicked
        infoTextMsg.color = new Color32(255, 255, 255, 255);
        infoTextMsg.text = "";

        switch (menuButton)
        {
            case ButtonType.NewGame:
                {
                    AuthManager.LoginWithCustomID();
                    break;
                }
            case ButtonType.ContGame:
                {
                    SceneChanger.sceneChangeEvent.Invoke(Scene.Loading);
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
                    break;
                }
            case ButtonType.SignUp:
                {
                    string email = emailInput.text;
                    string password = passwordInput.text;
                    string username = usernameInput.text;
                    AuthManager.RegisterUser(email, password, username);
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

    private void AuthManager_SuccessHandler(object sender, string authEvent)
    {
        infoTextMsg.color = new Color32(0, 255, 25, 255);
        infoTextMsg.text = authEvent + " successful";

        SceneChanger.sceneChangeEvent.Invoke(Scene.Loading);
    }

    private void AuthManager_ErrorHandler(object sender, string errorMsg)
    {
        infoTextMsg.color = new Color32(255, 110, 0, 255);
        infoTextMsg.text = errorMsg;
    }
}
