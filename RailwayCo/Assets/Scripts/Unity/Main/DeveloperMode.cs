using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;


public class DeveloperMode : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private int _expVal;
    [SerializeField] private int _coinVal;
    [SerializeField] private int _noteVal;
    [SerializeField] private int _normalCrateVal;
    [SerializeField] private int _specialCrateVal;

    [SerializeField] private GameManager _gameManager;

    private void Awake()
    {
        if (!_gameManager) Debug.LogError("Game Manager is not attached to the Developer Mode Script!");
    }

    public void AddToUserCurrency()
    {
        CurrencyManager currMgr = new();
        currMgr.CurrencyDict[CurrencyType.Coin] = _coinVal;
        currMgr.CurrencyDict[CurrencyType.Note] = _noteVal;
        currMgr.CurrencyDict[CurrencyType.NormalCrate] = _normalCrateVal;
        currMgr.CurrencyDict[CurrencyType.SpecialCrate] = _specialCrateVal;

        _gameManager.GameLogic.AddUserCurrencyManager(currMgr);
        this.GetComponent<LogicManager>().UpdateBottomUIStatsPanel();
    }
#endif
}
