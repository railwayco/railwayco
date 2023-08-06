using UnityEngine;


public class TrackManager : MonoBehaviour
{
    private LogicManager _logicMgr;
    private bool _isShowingTrackUnlock = false;

    public bool IsTrackUnlocked { get; private set; }
    public int PathCost { get; private set; }
    private int UnlockCostCrate { get; set; } // Brown Crates
    private int UnlockCostCoin { get; set; }

    ////////////////////////////////////////
    /// INITIALISATION PROCESSES
    ////////////////////////////////////////

    private void Awake()
    {
        _logicMgr = GameObject.Find("LogicManager").GetComponent<LogicManager>();
        if (!_logicMgr) Debug.LogError($"LogicManager is not present in the scene");

        foreach (Transform child in transform)
        {
            TrackController track = child.GetComponent<TrackController>();

            // Calculate Path Cost
            PathCost += track.PathCost;
            UnlockCostCoin += track.UnlockCostCoin;
            UnlockCostCrate += track.UnlockCostCrate;

            // Subscribe to Track events
            track.UnlockTrackEvent += Track_UnlockTrackEvent;
            track.ToggleShowUnlockEvent += Track_ToggleShowUnlockEvent;
        }

        SetInitialTrackStatus();
        UpdateTrackRender();
    }

    private void SetInitialTrackStatus()
    {
        OperationalStatus status = _logicMgr.GetTrackStatus(name);
        if (status == OperationalStatus.Open || status == OperationalStatus.Closed)
            IsTrackUnlocked = true;
        else if (status == OperationalStatus.Locked)
            IsTrackUnlocked = false;
    }

    ///////////////////////////////////////
    /// EVENT UPDATES
    ////////////////////////////////////////

    private void Track_ToggleShowUnlockEvent(object sender, string e)
    {
        if (!_isShowingTrackUnlock && !IsTrackUnlocked)
            TooltipManager.Show($"Cost: {UnlockCostCoin} coins, {UnlockCostCrate} brown crates ", "Unlock Tracks");
        else if (_isShowingTrackUnlock && !IsTrackUnlocked)
            TooltipManager.Hide();
        _isShowingTrackUnlock = !_isShowingTrackUnlock;
    }

    private void Track_UnlockTrackEvent(object sender, string e)
    {
        if (IsTrackUnlocked) return;

        CurrencyManager currMgr = new();
        currMgr.AddCurrency(CurrencyType.Coin, UnlockCostCoin);
        currMgr.AddCurrency(CurrencyType.NormalCrate, UnlockCostCrate);

        if (!_logicMgr.UnlockTracks(name, currMgr)) return;
        UpdateTrackStatus(true);
    }

    public void UpdateTrackStatus(bool isUnlocked)
    {
        IsTrackUnlocked = isUnlocked;
        UpdateTrackRender();
    }

    private void UpdateTrackRender()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<TrackController>().UpdateTrackRender(IsTrackUnlocked);
        }
    }

    ///////////////////////////////////////
    /// PUBLIC METHODS
    ///////////////////////////////////////

    public string GetLineName()
    {
        return transform.parent.gameObject.name;
    }
}
