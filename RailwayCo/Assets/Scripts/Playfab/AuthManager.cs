using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class AuthManager
{
    private string sessionTicket;
    private readonly GetPlayerCombinedInfoRequestParams playerReqParams = new()
    {
        GetPlayerProfile = true,
        GetPlayerStatistics = true,
        GetTitleData = true,
        GetUserAccountInfo = true,
        GetUserData = true,
        GetUserInventory = true,
        GetUserReadOnlyData = true,
        GetUserVirtualCurrency = true,
        PlayerStatisticNames = null,
        ProfileConstraints = null,
        TitleDataKeys = null,
        UserDataKeys = null,
        UserReadOnlyDataKeys = null
    };

    public string SessionTicket { get => sessionTicket; set => sessionTicket = value; }

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
        LoginType loginType = LoginType.CustomID;
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = playerReqParams
        };
        PlayFabClientAPI.LoginWithCustomID(
            request,
            (loginResult) => OnLoginSuccess(loginResult, loginType),
            (playFabError) => OnLoginError(playFabError, loginType));
    }

    public void LoginWithEmailAddress(string email, string password)
    {
        LoginType loginType = LoginType.EmailAddress;
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password,
            InfoRequestParameters = playerReqParams
        };
        PlayFabClientAPI.LoginWithEmailAddress(
            request,
            (loginResult) => OnLoginSuccess(loginResult, loginType),
            (playFabError) => OnLoginError(playFabError, loginType));
    }

    public void AddUsernamePassword(string email, string password, string username)
    {
        var request = new AddUsernamePasswordRequest
        {
            Email = email,
            Password = password,
            Username = username
        };
        PlayFabClientAPI.AddUsernamePassword(
            request,
            (aupResult) => OnAUPSuccess(aupResult),
            (playFabError) => OnAUPError(playFabError));
    }

    public void Logout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
    }

    void OnLoginSuccess(LoginResult loginResult, LoginType loginType)
    {
        Debug.Log(loginType.ToString() + " Account created successfully");
        SessionTicket = loginResult.SessionTicket;
    }

    void OnLoginError(PlayFabError playFabError, LoginType loginType)
    {
        Debug.Log("Error while logging in/creating " + loginType.ToString() + " Account");
        Debug.Log(playFabError.GenerateErrorReport());
    }

    void OnAUPSuccess(AddUsernamePasswordResult aupResult)
    {
        Debug.Log("AddUsernamePassword successful: Welcome " + aupResult.Username);
    }

    void OnAUPError(PlayFabError playFabError)
    {
        Debug.Log("Error while adding username and password");
        Debug.Log(playFabError.GenerateErrorReport());
    }
}

public enum LoginType
{
    CustomID,
    EmailAddress
}
