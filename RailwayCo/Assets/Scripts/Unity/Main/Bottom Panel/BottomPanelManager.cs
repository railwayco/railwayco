using UnityEngine;
using UnityEngine.UI;

public class BottomPanelManager : MonoBehaviour
{
    private static BottomPanelManager Instance { get; set; }

    [SerializeField] private Transform _statsPanel;

    private Text _expText;
    private Text _coinText;
    private Text _noteText;
    private Text _normalCrateText;
    private Text _specialCrateText;

    // Tells the camera manager how much bottom space the bottom panel is taking up.
    // Need it for proper camera click "isolation"
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        float bottomPanelHeightRatio = GetComponent<RectTransform>().rect.height / Screen.height;

        CameraManager.SetBottomPanelHeightRatio(bottomPanelHeightRatio);

        if (!Instance._statsPanel) Debug.LogError("Stats Panel not found");
        else
        {
            Instance._expText = Instance._statsPanel.Find("EXPText").GetComponent<Text>();
            Instance._coinText = Instance._statsPanel.Find("CoinText").GetComponent<Text>();
            Instance._noteText = Instance._statsPanel.Find("NoteText").GetComponent<Text>();
            Instance._normalCrateText = Instance._statsPanel.Find("NormalCrateText").GetComponent<Text>();
            Instance._specialCrateText = Instance._statsPanel.Find("SpecialCrateText").GetComponent<Text>();
        }
    }

    public static void SetUIStatsInformation(CurrencyManager currMgr, int exp)
    {
        int coinVal = currMgr.GetCurrency(CurrencyType.Coin);
        int noteVal = currMgr.GetCurrency(CurrencyType.Note);
        int normalCrateVal = currMgr.GetCurrency(CurrencyType.NormalCrate);
        int specialCrateVal = currMgr.GetCurrency(CurrencyType.SpecialCrate);

        Instance._expText.text = exp.ToString();
        Instance._coinText.text = coinVal.ToString();
        Instance._noteText.text = noteVal.ToString();
        Instance._normalCrateText.text = normalCrateVal.ToString();
        Instance._specialCrateText.text = specialCrateVal.ToString();
    }
}
