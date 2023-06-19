using PlayFab;

public class PlayfabManager
{
    private AuthManager authManager;
    private GameDataManager gameDataManager;

    public AuthManager AuthManager { get => authManager; private set => authManager = value; }
    public GameDataManager GameDataManager { get => gameDataManager; private set => gameDataManager = value; }

    public PlayfabManager()
    {
        PlayFabSettings.TitleId = "357DE";
        AuthManager = new();
        GameDataManager = new();
    }
}

