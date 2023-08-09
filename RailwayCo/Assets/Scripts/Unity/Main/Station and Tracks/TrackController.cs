using System;
using UnityEngine;

public class TrackController : MonoBehaviour
{
    [SerializeField] SpriteRenderer _trackSpriteRenderer;
    [SerializeField] SpriteRenderer _minimapSpriteRenderer;

    public event EventHandler UnlockTrack;
    public event EventHandler ToggleShowUnlock;

    public int PathCost { get; private set; }
    public int UnlockCostCrate { get; private set; } // Brown Crates
    public int UnlockCostCoin { get; private set; }

    ///////////////////////////////////////
    /// INITIALIZATION
    ////////////////////////////////////////

    private void Awake() => SetPathAndUnlockCost();

    private void SetPathAndUnlockCost()
    {
        string tagName = gameObject.tag;
        switch (tagName)
        {
            case "BridgeTD":
            case "BridgeLR":
            case "Track_LR":
            case "Track_TD":
                PathCost = 5;
                UnlockCostCrate = 1;
                UnlockCostCoin = 25;
                break;
            case "Track_Curved_RU":
            case "Track_Curved_RD":
            case "Track_Curved_LU":
            case "Track_Curved_LD":
                PathCost = 20;
                UnlockCostCrate = 5;
                UnlockCostCoin = 125;
                break;
            case "SlopeTD":
            case "SlopeLR":
                PathCost = 15;
                UnlockCostCrate = 2;
                UnlockCostCoin = 75;
                break;
            default:
                Debug.LogWarning($"{name}: Unhandled tag {tagName} to calculate path cost. Default to value of 5");
                PathCost = 5;
                UnlockCostCrate = 1;
                UnlockCostCoin = 25;
                break;
        }
    }

    ///////////////////////////////////////
    /// EVENT UPDATES
    ////////////////////////////////////////

    public void UpdateTrackRender(bool isTrackUnlocked)
    {
        Color trackColor = _trackSpriteRenderer.color;
        Color minimapMarkerColor;

        if (isTrackUnlocked)
        {
            trackColor.a = 1;
            minimapMarkerColor = new Color(1, 1, 1);
        }
        else
        {
            trackColor.a = 0.392f; //(100/255)
            minimapMarkerColor = new Color(0.4f, 0.4f, 0.4f); //0x666666
        }
        _trackSpriteRenderer.color = trackColor;
        _minimapSpriteRenderer.color = minimapMarkerColor;
    }

    ////////////////////////////////////////
    /// EVENT TRIGGERS
    ////////////////////////////////////////

    private void OnMouseEnter() => ToggleShowUnlock?.Invoke(this, EventArgs.Empty);

    private void OnMouseExit() => ToggleShowUnlock?.Invoke(this, EventArgs.Empty);

    public void OnMouseUpAsButton() => UnlockTrack?.Invoke(this, EventArgs.Empty);
}
