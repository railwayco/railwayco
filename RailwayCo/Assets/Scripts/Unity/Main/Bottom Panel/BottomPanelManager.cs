using UnityEngine;
using UnityEngine.UI;

public class BottomPanelManager : MonoBehaviour
{
    private Text _expText;
    private Text _coinText;
    private Text _noteText;
    private Text _normalCrateText;
    private Text _specialCrateText;

    // Tells the camera manager how much bottom space the bottom panel is taking up.
    // Need it for proper camera click "isolation"
    private void Awake()
    {
        float bottomPanelHeightRatio = GetComponent<RectTransform>().rect.height / Screen.height;

        CameraManager.SetBottomPanelHeightRatio(bottomPanelHeightRatio);

        Transform statsPanel = GameObject.Find("MainUI").transform.Find("BottomPanel").Find("UI_StatsPanel");
        if (!statsPanel) Debug.LogError("Stats Panel not found");
        else
        {
            _expText = statsPanel.Find("EXPText").GetComponent<Text>();
            _coinText = statsPanel.Find("CoinText").GetComponent<Text>();
            _noteText = statsPanel.Find("NoteText").GetComponent<Text>();
            _normalCrateText = statsPanel.Find("NormalCrateText").GetComponent<Text>();
            _specialCrateText = statsPanel.Find("SpecialCrateText").GetComponent<Text>();
        }
    }

    public void SetUIStatsInformation(CurrencyManager currMgr, int exp)
    {
        int coinVal = currMgr.GetCurrency(CurrencyType.Coin);
        int noteVal = currMgr.GetCurrency(CurrencyType.Note);
        int normalCrateVal = currMgr.GetCurrency(CurrencyType.NormalCrate);
        int specialCrateVal = currMgr.GetCurrency(CurrencyType.SpecialCrate);

        _expText.text = exp.ToString();
        _coinText.text = coinVal.ToString();
        _noteText.text = noteVal.ToString();
        _normalCrateText.text = normalCrateVal.ToString();
        _specialCrateText.text = specialCrateVal.ToString();
    }
}
