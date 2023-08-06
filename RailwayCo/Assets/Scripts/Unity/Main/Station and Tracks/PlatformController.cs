using System;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _trackPlatformSprite;
    [SerializeField] private SpriteRenderer _platformMinimapMarker;
    [SerializeField] private SpriteRenderer _trackMinimapMarker;

    private LogicManager _logicMgr;

    public Guid StationGuid { get; private set; } // Exposed to uniquely identify the station the platform is tagged to
    public Guid PlatformGuid { get; private set; }
    private GameObject _assocTrain; // Need the Train side to tell the platform that it has arrived

    // To keep track of who is to the left and right. Requires that the track be physically touching the platform for this to work.
    private GameObject _leftTrack = null;
    private GameObject _rightTrack = null;
    private GameObject _leftPlatform = null;
    private GameObject _rightPlatform = null;
    public int LeftStationNumber { get; private set; }
    public int RightStationNumber { get; private set; }
    public int CurrentStationNumber { get; private set; }

    public bool IsPlatformUnlocked { get; private set; }

    private readonly int _unlockCostCoin = 1500;
    private readonly int _unlockCostCrate = 20; // Purple crate

    /////////////////////////////////////
    /// INITIALISATION PROCESS
    /////////////////////////////////////
    private void Awake()
    {
        _logicMgr = GameObject.Find("GameManager").GetComponent<LogicManager>();
        if (!_logicMgr) Debug.LogError("LogicManager is not present in the scene");

        StationGuid = _logicMgr.GetStationGuid(gameObject.name);
        PlatformGuid = _logicMgr.GetPlatformGuid(gameObject.name);

        SetInitialPlatformStatus();
        UpdatePlatformRenderAndFunction();
    }

    private void SetInitialPlatformStatus()
    {
        OperationalStatus status = _logicMgr.GetPlatformStatus(PlatformGuid);
        if (status == OperationalStatus.Open || status == OperationalStatus.Closed)
            IsPlatformUnlocked = true;
        else if (status == OperationalStatus.Locked)
            IsPlatformUnlocked = false;
    }

    private void DetermineStationTrackReference(Collider track)
    {
        Vector2 platformPos = transform.position;
        Vector2 trackPos = track.transform.position;

        GameObject trackCollection = track.transform.parent.gameObject;
        string otherPlatformName = null;
        GameObject otherPlatform = null;

        string[] platformNames = trackCollection.name.Split('-');
        foreach (string name in platformNames)
        {
            if (name != this.name && otherPlatformName == null)
                otherPlatformName = name;
        }

        if (otherPlatformName == null)
            Debug.LogWarning("The other platform's name is never assigned!");
        else
            otherPlatform = GameObject.Find(otherPlatformName);

        if (CompareTag("PlatformLR"))
        {
            if (trackPos.x < platformPos.x)
            {
                _leftPlatform = otherPlatform;
                _leftTrack = trackCollection;
            }
            else if (trackPos.x > platformPos.x)
            {
                _rightPlatform = otherPlatform;
                _rightTrack = trackCollection;
            }
            else
                Debug.LogWarning("Please check Track and Platform alignment relationship (x)");
        }
        else if (CompareTag("PlatformTD"))
        {
            if (trackPos.y > platformPos.y)
            {
                _leftPlatform = otherPlatform;
                _leftTrack = trackCollection;
            }
            else if (trackPos.y < platformPos.y)
            {
                _rightPlatform = otherPlatform;
                _rightTrack = trackCollection;
            }
            else
                Debug.LogWarning("Please check Track and Platform alignment relationship (y)");
        }
        else
            Debug.LogWarning($"{name} has an unsupported tag attached to it!");

        ExtractStationNumberFromPlatforms();
    }

    private void ExtractStationNumberFromPlatforms()
    {
        CurrentStationNumber = LogicManager.GetStationPlatformNumbers(name).Item1;
        if (_leftPlatform) LeftStationNumber = LogicManager.GetStationPlatformNumbers(_leftPlatform.name).Item1;
        if (_rightPlatform) RightStationNumber = LogicManager.GetStationPlatformNumbers(_rightPlatform.name).Item1;
    }

    ///////////////////////////////////////
    /// EVENT UPDATES
    ////////////////////////////////////////

    // Called by the train when it stops at the platform and right when it moves
    // This is to allow for the correct cargo panel to be loaded.
    public void UpdateAssocTrain(GameObject train)
    {
        _assocTrain = train;
    }

    public void UpdatePlatformStatus(bool isUnlocked)
    {
        IsPlatformUnlocked = isUnlocked;
        UpdatePlatformRenderAndFunction();
    }

    private void UpdatePlatformRenderAndFunction()
    {
        Color track = _trackPlatformSprite.color;
        Color platform, minimapMarker;

        if (IsPlatformUnlocked)
        {
            track.a = 1;
            platform = Color.white;
            minimapMarker = Color.white;
        }
        else
        {
            track.a = 0.392f; //100/255
            platform = new Color(0.4f, 0.4f, 0.4f); //0x666666
            minimapMarker = new Color(0.4f, 0.4f, 0.4f); //0x666666
        }
        _trackPlatformSprite.color = track;
        _platformMinimapMarker.color = platform;
        _trackMinimapMarker.color = minimapMarker;
        // GetComponent<BoxCollider>().enabled = IsPlatformUnlocked;
    }

    ///////////////////////////////////////
    /// EVENT TRIGGERS
    ////////////////////////////////////////

    private void OnMouseEnter()
    {
        if (!IsPlatformUnlocked)
        {
            TooltipManager.Show($"Cost: {_unlockCostCoin} coins, {_unlockCostCrate} purple crates ", "Unlock Platform");
        }
    }
    private void OnMouseExit() => TooltipManager.Hide();

    private void OnMouseUpAsButton()
    {
        if (IsPlatformUnlocked)
            LoadCargoPanelViaPlatform();
        else
            ProcessPlatformUnlock();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Track_LR":
            case "Track_TD":
                DetermineStationTrackReference(other);
                break;
            case "Train":
                break;
            default:
                Debug.LogWarning($"Platform {name} detected unknown object with tag {other.tag}");
                break;
        }
    }

    //////////////////////////////////////////////////////
    // MISC PROCESSING FUNCTIONS
    //////////////////////////////////////////////////////
    private void ProcessPlatformUnlock()
    {
        CurrencyManager currMgr = new();
        currMgr.AddCurrency(CurrencyType.Coin, _unlockCostCoin);
        currMgr.AddCurrency(CurrencyType.SpecialCrate, _unlockCostCrate);

        if (!_logicMgr.UnlockPlatform(PlatformGuid, currMgr))
            return;
        UpdatePlatformStatus(true);
    }

    //////////////////////////////////////////////////////
    // PUBLIC FUNCTIONS
    //////////////////////////////////////////////////////

    public void FollowPlatform()
    {
        CameraManager.WorldCamFollowPlatform(gameObject);
    }

    public void LoadCargoPanelViaPlatform()
    {
        RightPanelManager.LoadCargoPanel(_assocTrain, gameObject, CargoTabOptions.Nil);
    }

    public bool IsLeftOrUpAccessible()
    {
        if (!_leftTrack || !_leftPlatform) return false;

        bool trackStatus = _leftTrack.GetComponent<TrackCollection>().IsTrackUnlocked;
        bool platformStatus = _leftPlatform.GetComponent<PlatformController>().IsPlatformUnlocked;
        return trackStatus && platformStatus;
    }

    public bool IsRightOrDownAccessible()
    {
        if (!_rightTrack || !_rightPlatform) return false;

        bool trackStatus = _rightTrack.GetComponent<TrackCollection>().IsTrackUnlocked;
        bool platformStatus = _rightPlatform.GetComponent<PlatformController>().IsPlatformUnlocked;
        return trackStatus && platformStatus;
    }

    public int GetLeftPathCost()
    {
        if (!_leftTrack) return 0;
        return _leftTrack.GetComponent<TrackCollection>().PathCost;
    }

    public int GetRightPathCost()
    {
        if (!_rightTrack) return 0;
        return _rightTrack.GetComponent<TrackCollection>().PathCost;
    }

    public string GetLineName()
    {
        if (_leftTrack)
            return _leftTrack.GetComponent<TrackCollection>().GetLineName();
        else if (_rightTrack)
            return _rightTrack.GetComponent<TrackCollection>().GetLineName();
        else
            return "LineUnknown";
    }
}
