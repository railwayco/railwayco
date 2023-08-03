using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserAuth : MonoBehaviour
{
    [SerializeField] private Button signInBtn;
    [SerializeField] private Button signUpBtn;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Button crossOutBtn;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField usernameInput;

    [SerializeField] private WelcomeScript welcomeScript;

    private bool _isSignIn = true;

    void Start()
    {
        signInBtn.onClick.AddListener(() => 
        {
            welcomeScript.ResetInfoTextMsg();
            string email = emailInput.text;
            string password = passwordInput.text;
            AuthManager.LoginWithEmailAddress(email, password);
        });
        signUpBtn.onClick.AddListener(() =>
        {
            welcomeScript.ResetInfoTextMsg();
            string email = emailInput.text;
            string password = passwordInput.text;
            string username = usernameInput.text;
            AuthManager.RegisterUser(email, password, username);
        });
        cancelBtn.onClick.AddListener(SwitchToMenu);
        crossOutBtn.onClick.AddListener(SwitchToMenu);
    }

    void Update() => ToggleButtons();

    private void ResetInputFields()
    {
        emailInput.text = "";
        passwordInput.text = "";
        usernameInput.text = "";
    }

    private void ToggleButtons()
    {
        signInBtn.gameObject.SetActive(_isSignIn);
        signUpBtn.gameObject.SetActive(!_isSignIn);
        usernameInput.gameObject.SetActive(!_isSignIn);
    }

    private void SwitchToMenu()
    {
        ResetInputFields();
        welcomeScript.SwitchToMenu();
    }

    public void SetupForm(bool isSignIn)
    {
        _isSignIn = isSignIn;
        ToggleButtons();
    }
}
