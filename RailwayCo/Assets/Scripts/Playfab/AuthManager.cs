using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class LoginManager : MonoBehaviour
{
    public Text emailText;
    public Text passwordText;
    public InputField emailInput;
    public InputField passwordInput;

    public void LoginWithCustomID()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnCustomIDLoginSuccess, OnCustomIDLoginError);
    }




    void OnCustomIDLoginSuccess(LoginResult loginResult)
    {
        Debug.Log("Custom ID Account created successfully");
    }

    void OnCustomIDLoginError(PlayFabError playFabError)
    {
        Debug.Log("Error while logging in/creating Custom ID Account");
        Debug.Log(playFabError.GenerateErrorReport());
    }

}
