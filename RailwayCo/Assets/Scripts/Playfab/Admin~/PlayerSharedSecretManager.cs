using UnityEngine;
using PlayFab;
using PlayFab.AdminModels;
using System.Collections.Generic;

public class PlayerSharedSecretManager
{
    private string secretKey;

    public void CreatePlayerSharedSecret(string SharedSecretFriendlyName)
    {
        var request = new CreatePlayerSharedSecretRequest
        {
            FriendlyName = SharedSecretFriendlyName
        };
        PlayFabAdminAPI.CreatePlayerSharedSecret(
            request,
            (result) => OnCreateSuccess(result),
            (playFabError) => OnError(playFabError));
    }

    public void DeletePlayerSharedSecret()
    {
        var request = new DeletePlayerSharedSecretRequest
        {
            SecretKey = secretKey
        };
        PlayFabAdminAPI.DeletePlayerSharedSecret(
            request,
            OnDeleteSuccess,
            OnError);
    }

    public void GetPlayerSharedSecrets()
    {
        var request = new GetPlayerSharedSecretsRequest();
        PlayFabAdminAPI.GetPlayerSharedSecrets(
            request,
            OnGetSecretsSuccess,
            OnError);
    }

    private void OnCreateSuccess(CreatePlayerSharedSecretResult result)
    {
        secretKey = result.SecretKey;
        Debug.Log("CreatePlayerSharedSecret successful: " + secretKey);
    }

    private void OnDeleteSuccess(DeletePlayerSharedSecretResult result)
    {
        Debug.Log("DeletePlayerSharedSecret successful");
    }

    private void OnGetSecretsSuccess(GetPlayerSharedSecretsResult result)
    {
        List<SharedSecret> sharedSecrets = result.SharedSecrets;
        foreach (SharedSecret secret in sharedSecrets)
        {
            Debug.Log(secret.ToJson());
        }
    }
    
    private void OnError(PlayFabError playFabError)
    {
        Debug.Log(playFabError.GenerateErrorReport());
    }
}
