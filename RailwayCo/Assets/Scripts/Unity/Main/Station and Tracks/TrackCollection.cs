using System;
using UnityEngine;

public class TrackCollection : MonoBehaviour
{
    private bool _isShowingTrackUnlock = false; // For Tooltip usage to toggle show and hide

    public bool IsTrackUnlocked { get; private set; }
    public int PathCost { get; private set; }
    private int UnlockCostCrate { get; set; } // Brown Crates
    private int UnlockCostCoin { get; set; }

    ////////////////////////////////////////
    /// INITIALISATION PROCESSES
    ////////////////////////////////////////

    private void Awake()
    {
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
        OperationalStatus status = TrackManager.GetTrackStatus(name);
        if (status == OperationalStatus.Open || status == OperationalStatus.Closed)
            IsTrackUnlocked = true;
        else if (status == OperationalStatus.Locked)
            IsTrackUnlocked = false;
    }

    ///////////////////////////////////////
    /// EVENT UPDATES
    ////////////////////////////////////////

    private void Track_ToggleShowUnlockEvent(object sender, EventArgs e)
    {
        if (!_isShowingTrackUnlock && !IsTrackUnlocked)
            TooltipManager.Show($"Cost: {UnlockCostCoin} coins, {UnlockCostCrate} brown crates ", "Unlock Tracks");
        else if (_isShowingTrackUnlock && !IsTrackUnlocked)
            TooltipManager.Hide();
        _isShowingTrackUnlock = !_isShowingTrackUnlock;
    }

    private void Track_UnlockTrackEvent(object sender, EventArgs e)
    {
        if (IsTrackUnlocked) return;

        CurrencyManager currMgr = new();
        currMgr.AddCurrency(CurrencyType.Coin, UnlockCostCoin);
        currMgr.AddCurrency(CurrencyType.NormalCrate, UnlockCostCrate);

        if (!TrackManager.UnlockTracks(name, currMgr)) return;
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

    public string GetLineName() => transform.parent.gameObject.name;
}
