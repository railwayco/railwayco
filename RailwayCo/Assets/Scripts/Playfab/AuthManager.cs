using System;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class AuthManager
{
    public static event EventHandler<string> SuccessHandler;
    public static event EventHandler<string> ErrorHandler;

    enum AuthEventType
    {
        LoginCustomID,
        LoginEmailAddress,
        AddUsernamePassword,
        RegisterUser
    }

    public static bool IsLoggedIn() => PlayFabClientAPI.IsClientLoggedIn();

    public static void LoginWithCustomID()
    {
        AuthEventType authEventType = AuthEventType.LoginCustomID;
        var request = new LoginWithCustomIDRequest
        {
            // CustomId = SystemInfo.deviceUniqueIdentifier,
            // HOTFIX: Temporary new game bypass
            CustomId = Guid.NewGuid().ToString(),
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(
            request,
            (result) => OnSuccess(authEventType),
            (playFabError) => OnError(authEventType, playFabError));
    }

    public static void LoginWithEmailAddress(string email, string password)
    {
        AuthEventType authEventType = AuthEventType.LoginEmailAddress;
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };
        PlayFabClientAPI.LoginWithEmailAddress(
            request,
            (result) => OnSuccess(authEventType),
            (playFabError) => OnError(authEventType, playFabError));
    }

    public static void AddUsernamePassword(string email, string password, string username)
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
            (result) => OnSuccess(authEventType),
            (playFabError) => OnError(authEventType, playFabError));
    }

    public static void RegisterUser(string email, string password, string username)
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
            (result) => OnSuccess(authEventType),
            (playFabError) => OnError(authEventType, playFabError));
    }

    public static void Logout() => PlayFabClientAPI.ForgetAllCredentials();

    private static void OnSuccess(AuthEventType authEventType)
    {
        string authEvent = authEventType.ToString();
        SuccessHandler?.Invoke(authEventType, authEvent);
        Debug.Log(authEvent + " successful");
    }

    private static void OnError(AuthEventType authEventType, PlayFabError playFabError)
    {
        string errorMsg = playFabError.GenerateErrorReport();
        ErrorHandler?.Invoke(authEventType, errorMsg);

        switch(authEventType)
        {
            case AuthEventType.AddUsernamePassword:
                {
                    Debug.LogError("Error while adding username and password");
                    break;
                }
            case AuthEventType.RegisterUser:
                {
                    Debug.LogError("Error while registering username and password");
                    break;
                }
            default:
                {
                    Debug.LogError("Error while logging in/creating " + authEventType.ToString() + " Account");
                    break;
                }
        }
        Debug.LogError(errorMsg);
    }
}
