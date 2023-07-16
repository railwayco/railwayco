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
        // Button functionality should only be available when the cargo is in the platform's associated station with a train inside.
        if (_currentTrainGUID == Guid.Empty || _currentStationGUID == Guid.Empty) return;
        if (!_logicMgr.ShiftCargoOnButtonClick(this.gameObject, _cargo, _currentTrainGUID, _currentStationGUID))
        {
            string eventType = "";
            CargoAssociation cargoAssoc = _cargo.CargoAssoc;
            if (cargoAssoc == CargoAssociation.Station || cargoAssoc == CargoAssociation.Yard)
                eventType = "Train capacity is full";
            else if (cargoAssoc == CargoAssociation.Train)
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
        string dest = "Station " + _logicMgr.GetIndividualStation(destStationGUID).Number.ToString();

        string cargoType = cargo.Type.ToString();
        string weight = cargo.Weight.ToString();
        string cargoDetail = cargoType + " (" + weight + " t)";

        CurrencyManager currMgr = cargo.CurrencyManager;
        int coinAmt = currMgr.GetCurrency(CurrencyType.Coin);
        int noteAmt = currMgr.GetCurrency(CurrencyType.Note);
        int nCrateAmt = currMgr.GetCurrency(CurrencyType.NormalCrate);
        int sCrateAmt = currMgr.GetCurrency(CurrencyType.SpecialCrate);

        this.transform.Find("CargoDetails").GetComponent<Text>().text = cargoDetail;
        this.transform.Find("Destination").GetComponent<Text>().text = dest;
        this.transform.Find("CoinAmt").GetComponent<Text>().text = coinAmt.ToString();
        this.transform.Find("NoteAmt").GetComponent<Text>().text = noteAmt.ToString();
        this.transform.Find("NormalCrateAmt").GetComponent<Text>().text = nCrateAmt.ToString();
        this.transform.Find("SpecialCrateAmt").GetComponent<Text>().text = sCrateAmt.ToString();
    }

}
