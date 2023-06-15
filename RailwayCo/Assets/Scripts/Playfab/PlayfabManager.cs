using PlayFab;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayfabScriptableObject", menuName = "ScriptableObjects/Playfab")]
public class PlayfabManager : ScriptableObject
{
    private AuthManager authManager;
    private GameDataManager gameDataManager;

    public AuthManager AuthManager { get => authManager; private set => authManager = value; }
    public GameDataManager GameDataManager { get => gameDataManager; private set => gameDataManager = value; }

    private void Awake()
    {
        PlayFabSettings.TitleId = "357DE";
        AuthManager = new();
        GameDataManager = new();
    }
}
