using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserAuth : MonoBehaviour
{
    [SerializeField] private Button _signInBtn;
    [SerializeField] private Button _signUpBtn;
    [SerializeField] private Button _cancelBtn;
    [SerializeField] private Button _crossOutBtn;
    [SerializeField] private TMP_InputField _emailInput;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private TMP_InputField _usernameInput;
    [SerializeField] private WelcomeScript _welcomeScript;

    private bool _isSignIn = true;

    private void Awake()
    {
        if (!_signInBtn) Debug.LogError("Sign In Button not attached to UserAuth");
        if (!_signUpBtn) Debug.LogError("Sign Up Button not attached to UserAuth");
        if (!_cancelBtn) Debug.LogError("Cancel Button not attached to UserAuth");
        if (!_crossOutBtn) Debug.LogError("Cross Out Button not attached to UserAuth");
        if (!_emailInput) Debug.LogError("Email Input not attached to UserAuth");
        if (!_passwordInput) Debug.LogError("Password Input not attached to UserAuth");
        if (!_usernameInput) Debug.LogError("Username Input not attached to UserAuth");
        if (!_welcomeScript) Debug.LogError("Welcome Script not attached to UserAuth");
    }

    private void Start()
    {
        _signInBtn.onClick.AddListener(() =>
        {
            _welcomeScript.ResetInfoTextMsg();
            string email = _emailInput.text;
            string password = _passwordInput.text;
            AuthManager.LoginWithEmailAddress(email, password);
        });
        _signUpBtn.onClick.AddListener(() =>
        {
            _welcomeScript.ResetInfoTextMsg();
            string email = _emailInput.text;
            string password = _passwordInput.text;
            string username = _usernameInput.text;
            AuthManager.RegisterUser(email, password, username);
        });
        _cancelBtn.onClick.AddListener(SwitchToMenu);
        _crossOutBtn.onClick.AddListener(SwitchToMenu);
    }

    private void Update() => ToggleButtons();

    private void ResetInputFields()
    {
        _emailInput.text = "";
        _passwordInput.text = "";
        _usernameInput.text = "";
    }

    private void ToggleButtons()
    {
        _signInBtn.gameObject.SetActive(_isSignIn);
        _signUpBtn.gameObject.SetActive(!_isSignIn);
        _usernameInput.gameObject.SetActive(!_isSignIn);
    }

    private void SwitchToMenu()
    {
        ResetInputFields();
        _welcomeScript.SwitchToMenu();
    }

    public void SetupForm(bool isSignIn)
    {
        _isSignIn = isSignIn;
        ToggleButtons();
    }
}
