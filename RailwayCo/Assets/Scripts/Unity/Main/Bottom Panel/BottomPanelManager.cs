using UnityEngine;
using UnityEngine.UI;

public class BottomPanelManager : MonoBehaviour
{
    // Tells the camera manager how much bottom space the bottom panel is taking up.
    // Need it for proper camera click "isolation"
    private void Awake()
    {
        GameObject mainUI = GameObject.Find("MainUI");
        Vector2 refReso = mainUI.GetComponent<CanvasScaler>().referenceResolution;
        float bottomPanelHeightRatio = this.GetComponent<RectTransform>().rect.height/ refReso[1];

        CameraManager camMgr = GameObject.Find("CameraList").GetComponent<CameraManager>();
        camMgr.SetBottomPanelHeightRatio(bottomPanelHeightRatio);
    }


    public void SetUIStatsInformation(CurrencyManager currMgr, int exp)
    {
        int coinVal = currMgr.GetCurrency(CurrencyType.Coin);
        int noteVal = currMgr.GetCurrency(CurrencyType.Note);
        int normalCrateVal = currMgr.GetCurrency(CurrencyType.NormalCrate);
        int specialCrateVal = currMgr.GetCurrency(CurrencyType.SpecialCrate);

        Transform statsPanel = GameObject.Find("MainUI").transform.Find("BottomPanel").Find("UI_StatsPanel");
        statsPanel.Find("EXPText").GetComponent<Text>().text = exp.ToString();
        statsPanel.Find("CoinText").GetComponent<Text>().text = coinVal.ToString();
        statsPanel.Find("NoteText").GetComponent<Text>().text = noteVal.ToString();
        statsPanel.Find("NormalCrateText").GetComponent<Text>().text = normalCrateVal.ToString();
        statsPanel.Find("SpecialCrateText").GetComponent<Text>().text = specialCrateVal.ToString();
    }
}
