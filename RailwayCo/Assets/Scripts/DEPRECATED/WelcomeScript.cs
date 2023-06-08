using System.Collections;
using System.Collections.Generic;
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
        switch(menuButton)
        {
            case ButtonType.NewGame:
                {
                    Debug.Log("New Game button clicked!");
                    break;
                }
            case ButtonType.ContGame:
                {
                    Debug.Log("Continue Game button clicked!");
                    break;
                }
            case ButtonType.Login:
                {
                    menuPanel.SetActive(false);
                    formPanel.SetActive(true);
                    signInBtn.gameObject.SetActive(true);
                    signUpBtn.gameObject.SetActive(false);
                    Debug.Log("Login button clicked!");
                    break;
                }
            case ButtonType.Logout:
                {
                    isLoggedIn = false;
                    Update();
                    Debug.Log("Logout button clicked!");
                    break;
                }
            case ButtonType.CreateAcc:
                {
                    menuPanel.SetActive(false);
                    formPanel.SetActive(true);
                    signInBtn.gameObject.SetActive(false);
                    signUpBtn.gameObject.SetActive(true);
                    Debug.Log("Create Account button clicked!");
                    break;
                }
            case ButtonType.Settings:
                {
                    Debug.Log("Settings button clicked!");
                    break;
                }
            case ButtonType.SignIn:
                {
                    isLoggedIn = true;
                    formPanel.SetActive(false);
                    menuPanel.SetActive(true);
                    Update();
                    Debug.Log("Sign In button clicked!");
                    break;
                }
            case ButtonType.SignUp:
                {
                    formPanel.SetActive(false);
                    menuPanel.SetActive(true);

                    // TODO: Check if sign up successful

                    Debug.Log("Sign Up button clicked!");
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
    SignUp
}
