using UnityEngine;

public class UserManager : MonoBehaviour
{
    private static UserManager Instance { get; set; }

    [SerializeField] private GameLogic _gameLogic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        if (!Instance._gameLogic) Debug.LogError("Game Logic is not attached to the logic manager!");
    }

    private void Start() => UpdateUserStatsPanel();

    public static void UpdateUserStatsPanel()
    {
        int exp = Instance._gameLogic.GetUserExperiencePoints();
        CurrencyManager currMgr = GetUserCurrencyStats();
        BottomPanelManager.SetUIStatsInformation(currMgr, exp);
    }

    public static CurrencyManager GetUserCurrencyStats() => Instance._gameLogic.GetUserCurrencyManager();
}
