using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CargoDetailButton : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] private Button _cargoDetailButton;
    private LogicManager _logicMgr;
    private Cargo _cargo;
    private Guid _currentTrainGUID;
    private Guid _currentStationGUID;

    // Setup for the Cargo detail button
    public void SetCargoInformation(Cargo cargo, Guid trainguid, Guid stationguid, bool disableCargoDetailButton) 
    {
        _cargo = cargo;
        _currentTrainGUID = trainguid;
        _currentStationGUID = stationguid;
        PopulateCargoInformation(cargo, disableCargoDetailButton);
    }

    /////////////////////////////////////////////////////
    // INITIALISATION
    /////////////////////////////////////////////////////
    private void Awake()
    {
        if (!_cargoDetailButton) Debug.LogError("Cargo Detail button did not reference itself");
        _cargoDetailButton.onClick.AddListener(OnButtonClicked);

        GameObject lgMgr = GameObject.Find("LogicManager");
        if (!lgMgr) Debug.LogError("Unable to find the Logic Manager");
        _logicMgr = lgMgr.GetComponent<LogicManager>();
        if (!_logicMgr) Debug.LogError("Unable to find the Logic Manager Script");

    }

    private void OnButtonClicked()
    {
        // Button functionality should only be available when the cargo is in the station with a train inside.
        if (_currentTrainGUID == Guid.Empty || _currentStationGUID == Guid.Empty) return;
        if (!_logicMgr.ShiftCargoOnButtonClick(this.gameObject, _cargo, _currentTrainGUID, _currentStationGUID))
        {
            string eventType = "";
            CargoAssociation cargoAssoc = _cargo.CargoAssoc;
            if (cargoAssoc == CargoAssociation.STATION || cargoAssoc == CargoAssociation.YARD)
                eventType = "Train capacity is full";
            else if (cargoAssoc == CargoAssociation.TRAIN)
                eventType = "Yard capacity is full";
            TooltipManager.Show(eventType, "Error");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Hide();
    }

    private void PopulateCargoInformation(Cargo cargo, bool disableCargoDetailButton)
    {
        if (disableCargoDetailButton)
        {
            this.GetComponent<Button>().enabled = false;
        }

        Guid destStationGUID = cargo.TravelPlan.DestinationStation;
        string dest = _logicMgr.GetIndividualStation(destStationGUID).Name;

        string cargoType = cargo.Type.ToString();
        string weight = ((int)(cargo.Weight)).ToString();
        string cargoDetail = cargoType + " (" + weight + " t)";

        CurrencyManager currMgr = cargo.CurrencyManager;
        Currency currrency;

        currMgr.CurrencyDict.TryGetValue(CurrencyType.Coin, out currrency);
        string coinAmt = currrency.CurrencyValue.ToString();

        currMgr.CurrencyDict.TryGetValue(CurrencyType.Note, out currrency);
        string noteAmt = currrency.CurrencyValue.ToString();

        currMgr.CurrencyDict.TryGetValue(CurrencyType.NormalCrate, out currrency);
        string nCrateAmt = currrency.CurrencyValue.ToString();

        currMgr.CurrencyDict.TryGetValue(CurrencyType.SpecialCrate, out currrency);
        string sCrateAmt = currrency.CurrencyValue.ToString();

        this.transform.Find("CargoDetails").GetComponent<Text>().text = cargoDetail;
        this.transform.Find("Destination").GetComponent<Text>().text = dest;
        this.transform.Find("CoinAmt").GetComponent<Text>().text = coinAmt;
        this.transform.Find("NoteAmt").GetComponent<Text>().text = noteAmt;
        this.transform.Find("NormalCrateAmt").GetComponent<Text>().text = nCrateAmt;
        this.transform.Find("SpecialCrateAmt").GetComponent<Text>().text = sCrateAmt;
    }

}
