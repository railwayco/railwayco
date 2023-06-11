using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class AuthManager
{
    private string sessionTicket;
    private EncryptionManager encryptionManager;

    public string SessionTicket { get => sessionTicket; set => sessionTicket = value; }
    private EncryptionManager EncryptionManager { get => encryptionManager; set => encryptionManager = value; }

    public AuthManager()
    {
        // TODO: Future additional platform support
            // Fetch exisiting SessionTicket from local database
            // PlayFabClientAPI.IsClientLoggedIn()
            // Sessions only persist for 24 hours
        SessionTicket = "";
    }

    public bool IsLoggedIn() => PlayFabClientAPI.IsClientLoggedIn();

    public void LoginWithCustomID()
    {
        AuthEventType authEventType = AuthEventType.LoginCustomID;
        var requestUnencrypted = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        string requestEncrypted = EncryptionManager.EncryptRequest(requestUnencrypted);
        var request = new LoginWithCustomIDRequest
        {
            EncryptedRequest = requestEncrypted
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
        var requestUnencrypted = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            Username = username
        };

        string requestEncrypted = EncryptionManager.EncryptRequest(requestUnencrypted);
        var request = new RegisterPlayFabUserRequest
        {
            EncryptedRequest = requestEncrypted
        };

        PlayFabClientAPI.RegisterPlayFabUser(
            request,
            (result) => OnSuccess(authEventType, result),
            (playFabError) => OnError(authEventType, playFabError));
    }

    public void Logout() => PlayFabClientAPI.ForgetAllCredentials();

    private void OnSuccess(AuthEventType authEventType, object result)
    {
        Debug.Log(authEventType.ToString() + " successful");
        if (authEventType is AuthEventType.LoginCustomID
            or AuthEventType.LoginEmailAddress)
        {
            sessionTicket = ((LoginResult)result).SessionTicket;
        }
    }

    private void OnError(AuthEventType authEventType, PlayFabError playFabError)
    {
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
        Debug.Log(playFabError.GenerateErrorReport());
    }
}

public enum AuthEventType
{
    LoginCustomID,
    LoginEmailAddress,
    AddUsernamePassword,
    RegisterUser
}
