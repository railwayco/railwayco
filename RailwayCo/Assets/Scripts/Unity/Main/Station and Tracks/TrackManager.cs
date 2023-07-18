using UnityEngine;


public class TrackManager : MonoBehaviour
{
    private LogicManager _logicMgr;

    public int PathCost { get; private set; }
    public int UnlockCostCrate { get; private set; } // Brown Crates
    public int UnlockCostCoin { get; private set; }
    public bool IsTrackUnlocked { get; private set; }


    ////////////////////////////////////////
    /// INITIALISATION PROCESSES
    ////////////////////////////////////////

    private void Awake()
    {
        _logicMgr = GameObject.Find("LogicManager").GetComponent<LogicManager>();
        if (!_logicMgr) Debug.LogError($"LogicManager is not present in the scene");

        CalculatePathCost();
        SetInitialTrackStatus();
        UpdateTrackRender();
    }

    private void CalculatePathCost()
    {
        int numTracks = this.transform.childCount;
        for (int i = 0; i < numTracks; i++)
        {
            Transform child = this.transform.GetChild(i);
            string tagName = child.gameObject.tag;

            switch (tagName)
            {
                case "BridgeTD":
                case "BridgeLR":
                case "Track_LR":
                case "Track_TD":
                    PathCost += 5;
                    UnlockCostCrate += 1;
                    UnlockCostCoin += 25;
                    break;
                case "Track_Curved_RU":
                case "Track_Curved_RD":
                case "Track_Curved_LU":
                case "Track_Curved_LD":
                    PathCost += 20;
                    UnlockCostCrate += 5;
                    UnlockCostCoin += 125;
                    break;
                case "SlopeTD":
                case "SlopeLR":
                    PathCost += 15;
                    UnlockCostCrate += 2;
                    UnlockCostCoin += 75;
                    break;
                default:
                    Debug.LogWarning($"{this.name}: Unhandled tag {tagName} for child {child.name} for the track manager to calculate path cost. Default to value of 5");
                    PathCost += 5;
                    UnlockCostCrate += 1;
                    UnlockCostCoin += 25;
                    break;
            }
        }
    }

    private void SetInitialTrackStatus()
    {
        string platformConnectionName = name;
        OperationalStatus status = _logicMgr.GetTrackStatus(platformConnectionName);

        if (status == OperationalStatus.Open)
            IsTrackUnlocked = true;
        else if (status == OperationalStatus.Locked)
            IsTrackUnlocked = false;
        else if (status == OperationalStatus.Closed)
            IsTrackUnlocked = true;
    }



    ///////////////////////////////////////
    /// EVENT UPDATES
    ////////////////////////////////////////

    public void UpdateTrackStatus(bool isUnlocked)
    {
        IsTrackUnlocked = isUnlocked;
        UpdateTrackRender();
    }

    private void UpdateTrackRender()
    {
        int numTracks = this.transform.childCount;
        for (int i = 0; i < numTracks; i++)
        {
            Transform child = this.transform.GetChild(i);
            Color trackColor = child.GetComponent<SpriteRenderer>().color;
            Transform minimapMarker = child.Find("MinimapMarker");

            if (IsTrackUnlocked)
            {
                trackColor.a = 1;
                child.GetComponent<SpriteRenderer>().color = trackColor;
                minimapMarker.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            }
            else
            {
                trackColor.a = 0.392f; //(100/255)
                child.GetComponent<SpriteRenderer>().color = trackColor;
                minimapMarker.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f); //0x666666
            }
        }
    }

    ///////////////////////////////////////
    /// EVENT TRIGGERS
    ////////////////////////////////////////
    
    public void ProcessTrackUnlock()
    {
        CurrencyManager currMgr = new();
        currMgr.AddCurrency(CurrencyType.Coin, UnlockCostCoin);
        currMgr.AddCurrency(CurrencyType.NormalCrate, UnlockCostCrate);

        if (!_logicMgr.UnlockTracks(this.name, currMgr))
            return;
        UpdateTrackStatus(true);
    }
}
