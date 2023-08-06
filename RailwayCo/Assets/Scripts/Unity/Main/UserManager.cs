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

    private void Start()
    {
        UpdateBottomUIStatsPanel();
    }

    public static void UpdateBottomUIStatsPanel()
    {
        int exp = Instance._gameLogic.GetUserExperiencePoints();
        CurrencyManager currMgr = GetUserCurrencyStats();
        BottomPanelManager bpm = GameObject.Find("MainUI").transform.Find("BottomPanel").GetComponent<BottomPanelManager>();
        bpm.SetUIStatsInformation(currMgr, exp);
    }

    public static CurrencyManager GetUserCurrencyStats() => Instance._gameLogic.GetUserCurrencyManager();
}
