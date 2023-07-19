using UnityEngine;


public class DeveloperMode : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private int _expVal;
    [SerializeField] private int _coinVal;
    [SerializeField] private int _noteVal;
    [SerializeField] private int _normalCrateVal;
    [SerializeField] private int _specialCrateVal;

    [SerializeField] private GameLogic _gameLogic;

    private void Awake()
    {
        if (!_gameLogic) Debug.LogError("Game Logic is not attached to the Developer Mode Script!");
    }

    public void AddToUserCurrency()
    {
        CurrencyManager currMgr = new();
        currMgr.CurrencyDict[CurrencyType.Coin] = _coinVal;
        currMgr.CurrencyDict[CurrencyType.Note] = _noteVal;
        currMgr.CurrencyDict[CurrencyType.NormalCrate] = _normalCrateVal;
        currMgr.CurrencyDict[CurrencyType.SpecialCrate] = _specialCrateVal;

        _gameLogic.AddUserCurrencyManager(currMgr);
        this.GetComponent<LogicManager>().UpdateBottomUIStatsPanel();
    }
#endif
}
