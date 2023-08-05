using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CargoDetailButton : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] private Button _cargoDetailButton;
    [SerializeField] private Text _cargoDetails;
    [SerializeField] private Text _destination;
    [SerializeField] private Text _coinAmt;
    [SerializeField] private Text _noteAmt;
    [SerializeField] private Text _normalCrateAmt;
    [SerializeField] private Text _specialCrateAmt;

    private CargoPanelManager _cargoPanelMgr;
    private Cargo _cargo;

    // Setup for the Cargo detail button
    public void SetCargoInformation(CargoPanelManager cargoPanelMgr, Cargo cargo, bool disableButton) 
    {
        _cargoPanelMgr = cargoPanelMgr;
        _cargo = cargo;
        PopulateCargoInformation();
        GetComponent<Button>().enabled = !disableButton;
    }

    /////////////////////////////////////////////////////
    // INITIALISATION
    /////////////////////////////////////////////////////
    private void Awake()
    {
        if (!_cargoDetailButton) Debug.LogError("Cargo Detail button did not reference itself");
        _cargoDetailButton.onClick.AddListener(OnButtonClicked);

        if (!_cargoDetails) Debug.LogError("Cargo Details Text not attached");
        if (!_destination) Debug.LogError("Destination Text not attached");
        if (!_coinAmt) Debug.LogError("Coin Amt Text button not attached");
        if (!_noteAmt) Debug.LogError("Note Amt Text not attached");
        if (!_normalCrateAmt) Debug.LogError("Normal Crate Amt Text not attached");
        if (!_specialCrateAmt) Debug.LogError("Special Crate Amt Text not attached");
    }

    private void OnButtonClicked()
    {
        if (!_cargoPanelMgr.MoveCargoBetweenTrainAndStation(_cargo))
        {
            string eventType = "";
            CargoAssociation cargoAssoc = _cargo.CargoAssoc;
            if (cargoAssoc == CargoAssociation.Station || cargoAssoc == CargoAssociation.Yard)
                eventType = "Train capacity is full";
            else if (cargoAssoc == CargoAssociation.Train)
                eventType = "Yard capacity is full";
            TooltipManager.Show(eventType, "Error");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Hide();
    }

    private void PopulateCargoInformation()
    {
        int stationNum = _cargoPanelMgr.GetStationNum(_cargo.TravelPlan.DestinationStation);
        string dest = $"Station {stationNum}";
        string cargoType = _cargo.Type.ToString();
        string weight = _cargo.Weight.ToString();
        string cargoDetail = $"{cargoType} ({weight} t)";

        CurrencyManager currMgr = _cargo.CurrencyManager;
        int coinAmt = currMgr.GetCurrency(CurrencyType.Coin);
        int noteAmt = currMgr.GetCurrency(CurrencyType.Note);
        int nCrateAmt = currMgr.GetCurrency(CurrencyType.NormalCrate);
        int sCrateAmt = currMgr.GetCurrency(CurrencyType.SpecialCrate);

        _cargoDetails.text = cargoDetail;
        _destination.text = dest;
        _coinAmt.text = coinAmt.ToString();
        _noteAmt.text = noteAmt.ToString();
        _normalCrateAmt.text = nCrateAmt.ToString();
        _specialCrateAmt.text = sCrateAmt.ToString();
    }
}
