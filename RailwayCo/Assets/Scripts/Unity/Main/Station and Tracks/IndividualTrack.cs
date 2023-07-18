using UnityEngine;

public class IndividualTrack : MonoBehaviour
{
    private TrackManager _trackMgr;

    ///////////////////////////////////////
    /// INITIALIZATION
    ////////////////////////////////////////

    private void Awake()
    {
        _trackMgr = this.transform.parent.GetComponent<TrackManager>();
        if (!_trackMgr) Debug.LogError("Track Manager not found!");
    }

    ///////////////////////////////////////
    /// EVENT TRIGGERS
    ////////////////////////////////////////
    private void OnMouseEnter()
    {
        if (!_trackMgr.IsTrackUnlocked)
        {
            int coinCost = _trackMgr.UnlockCostCoin;
            int normalCrateCost = _trackMgr.UnlockCostCrate;
            TooltipManager.Show($"Cost: {coinCost} coins, {normalCrateCost} brown crates ", "Unlock Tracks");
        }
    }

    private void OnMouseExit()
    {
        TooltipManager.Hide();
    }

    public void OnMouseUpAsButton()
    {
        if (!_trackMgr.IsTrackUnlocked)
        {
            _trackMgr.ProcessTrackUnlock();
        }
    }
}
