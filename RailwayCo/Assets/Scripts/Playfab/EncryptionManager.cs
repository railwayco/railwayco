using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.PfEditor.Json;

public class EncryptionManager
{
    private string sharedSecret = "HMEGNIZMZBPKEMQMANN9DHPWX8NOYMWRMBCIX9A8WWZBWB8YO4"; // RailwayCoSharedSecret
    private byte[] cspBlob;

    public string SharedSecret { get => sharedSecret; }
    private byte[] CspBlob { get => cspBlob; set => cspBlob = value; }

    public EncryptionManager()
    {
        _GetTitlePublicKey();
    }

    private void _GetTitlePublicKey()
    {
        var request = new GetTitlePublicKeyRequest
        {
            TitleSharedSecret = SharedSecret
        };
        PlayFabClientAPI.GetTitlePublicKey(
            request,
            OnGetTitlePublicKeySuccess,
            OnError);
    }

    public string EncryptRequest(object toJsonify)
    {
        string unencryptedPayload = JsonWrapper.SerializeObject(toJsonify);
        string encryptedPayload;
        using (var rsa = new System.Security.Cryptography.RSACryptoServiceProvider())
        {
            rsa.ImportCspBlob(CspBlob);
            var bytesToEncrypt = System.Text.Encoding.UTF8.GetBytes(unencryptedPayload);
            var encryptedBytes = rsa.Encrypt(bytesToEncrypt, true);
            encryptedPayload = System.Convert.ToBase64String(encryptedBytes);
        }
        return encryptedPayload;
    }

    private void OnGetTitlePublicKeySuccess(GetTitlePublicKeyResult result)
    {
        CspBlob = System.Convert.FromBase64String(result.RSAPublicKey);
        Debug.Log("GetTitlePublicKey successful");
    }

    private void OnError(PlayFabError playFabError)
    {
        Debug.Log(playFabError.GenerateErrorReport());
    }
}
