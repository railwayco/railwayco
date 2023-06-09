using UnityEngine;
using UnityEngine.UI;

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

    private bool isLoggedIn = false; // TODO: Check if user is logged in

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

    public void OnButtonClicked(ButtonType menuButton)
    {
        Debug.Log(menuButton.ToString() + " button clicked!");

        switch (menuButton)
        {
            case ButtonType.NewGame:
                {
                    // Scene manager to game scene
                    break;
                }
            case ButtonType.ContGame:
                {
                    // Scene manager to game scene
                    break;
                }
            case ButtonType.Login:
                {
                    menuPanel.SetActive(false);
                    formPanel.SetActive(true);
                    signInBtn.gameObject.SetActive(true);
                    signUpBtn.gameObject.SetActive(false);
                    break;
                }
            case ButtonType.Logout:
                {
                    isLoggedIn = false;
                    Update();
                    break;
                }
            case ButtonType.CreateAcc:
                {
                    menuPanel.SetActive(false);
                    formPanel.SetActive(true);
                    signInBtn.gameObject.SetActive(false);
                    signUpBtn.gameObject.SetActive(true);
                    break;
                }
            case ButtonType.Settings:
                {
                    // TODO: Implement settings
                    break;
                }
            case ButtonType.SignIn:
                {
                    // TODO: Check if sign in successful
                    
                    isLoggedIn = true;
                    formPanel.SetActive(false);
                    menuPanel.SetActive(true);
                    Update();
                    break;
                }
            case ButtonType.SignUp:
                {
                    // TODO: Check if sign up successful

                    formPanel.SetActive(false);
                    menuPanel.SetActive(true);
                    break;
                }
            case ButtonType.Cancel:
                {
                    formPanel.SetActive(false);
                    menuPanel.SetActive(true);
                    break;
                }
            case ButtonType.CrossOut:
                {
                    formPanel.SetActive(false);
                    menuPanel.SetActive(true);
                    break;
                }
        }
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
