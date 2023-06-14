using System;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class AuthManager
{
    private string sessionTicket;
    
    public event EventHandler<string> SuccessHandler;
    public event EventHandler<string> ErrorHandler;
    public string SessionTicket { get => sessionTicket; private set => sessionTicket = value; }

    enum AuthEventType
    {
        LoginCustomID,
        LoginEmailAddress,
        AddUsernamePassword,
        RegisterUser
    }

    public AuthManager()
    {
        // TODO: Future additional platform support
            // Fetch exisiting SessionTicket from local database
            // PlayFabClientAPI.IsClientLoggedIn()
            // Sessions only persist for 24 hours
        SessionTicket = "";
        PlayFabSettings.TitleId = "357DE";
    }

    public bool IsLoggedIn() => PlayFabClientAPI.IsClientLoggedIn();

    public void LoginWithCustomID()
    {
        AuthEventType authEventType = AuthEventType.LoginCustomID;
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(
            request,
            (result) => OnSuccess(authEventType, result),
            (playFabError) => OnError(authEventType, playFabError));
    }

    public void LoginWithEmailAddress(string email, string password)
    {
        AuthEventType authEventType = AuthEventType.LoginEmailAddress;
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };
        PlayFabClientAPI.LoginWithEmailAddress(
            request,
            (result) => OnSuccess(authEventType, result),
            (playFabError) => OnError(authEventType, playFabError));
    }

    public void AddUsernamePassword(string email, string password, string username)
    {
        AuthEventType authEventType = AuthEventType.AddUsernamePassword;
        var request = new AddUsernamePasswordRequest
        {
            Email = email,
            Password = password,
            Username = username
        };
        PlayFabClientAPI.AddUsernamePassword(
            request,
            (result) => OnSuccess(authEventType, result),
            (playFabError) => OnError(authEventType, playFabError));
    }

    public void RegisterUser(string email, string password, string username)
    {
        AuthEventType authEventType = AuthEventType.RegisterUser;
        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            Username = username
        };
        PlayFabClientAPI.RegisterPlayFabUser(
            request,
            (result) => OnSuccess(authEventType, result),
            (playFabError) => OnError(authEventType, playFabError));
    }

    public void Logout() => PlayFabClientAPI.ForgetAllCredentials();

    private void OnSuccess(AuthEventType authEventType, object result)
    {
        string authEvent = authEventType.ToString();
        SuccessHandler?.Invoke(this, authEvent);

        Debug.Log(authEvent + " successful");
        if (authEventType is AuthEventType.LoginCustomID
            or AuthEventType.LoginEmailAddress)
        {
            SessionTicket = ((LoginResult)result).SessionTicket;
        }
    }

    private void OnError(AuthEventType authEventType, PlayFabError playFabError)
    {
        string errorMsg = playFabError.GenerateErrorReport();
        ErrorHandler?.Invoke(this, errorMsg);

        switch(authEventType)
        {
            case AuthEventType.AddUsernamePassword:
                {
                    Debug.Log("Error while adding username and password");
                    break;
                }
            case AuthEventType.RegisterUser:
                {
                    Debug.Log("Error while registering username and password");
                    break;
                }
            default:
                {
                    Debug.Log("Error while logging in/creating " + authEventType.ToString() + " Account");
                    break;
                }
        }
        Debug.Log(errorMsg);
    }
}
