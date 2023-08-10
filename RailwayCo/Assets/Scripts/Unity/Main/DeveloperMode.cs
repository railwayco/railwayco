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

    private GameObject _dummyTrain;
    private GameObject _anotherTrain;

    private void Awake()
    {
        if (!_gameLogic) Debug.LogError("Game Logic is not attached to the Developer Mode Script!");

        GameObject[] trains = GameObject.FindGameObjectsWithTag("Train");
        if (trains.Length < 2)
        {
            _dummyTrain = null;
        }
        else
        {
            _dummyTrain = trains[0];
            _anotherTrain = trains[1];
        }

    }

    public void AddToUserCurrency()
    {
        CurrencyManager currMgr = new();
        currMgr.AddCurrency(CurrencyType.Coin, _coinVal);
        currMgr.AddCurrency(CurrencyType.Note, _noteVal);
        currMgr.AddCurrency(CurrencyType.NormalCrate, _normalCrateVal);
        currMgr.AddCurrency(CurrencyType.SpecialCrate, _specialCrateVal);

        _gameLogic.AddUserCurrencyManager(currMgr);
        UserManager.UpdateUserStatsPanel();
    }

    public void SetToUserCurrency()
    {
        CurrencyManager userCurrMgr = _gameLogic.GetUserCurrencyManager();
        _gameLogic.RemoveUserCurrencyManager(userCurrMgr);
        AddToUserCurrency();
    }

    public void TriggerTrainCollisionEvent()
    {
        if (!_dummyTrain)
        {
            Debug.Log("Not enough Active Trains in the hierarchy");
            return;
        }
        GameManager.ActivateCollisionPopup(_dummyTrain, _anotherTrain);
    }
#endif
}
