using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CargoDetailButton : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] private Button _cargoDetailButton;
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

        transform.Find("CargoDetails").GetComponent<Text>().text = cargoDetail;
        transform.Find("Destination").GetComponent<Text>().text = dest;
        transform.Find("CoinAmt").GetComponent<Text>().text = coinAmt.ToString();
        transform.Find("NoteAmt").GetComponent<Text>().text = noteAmt.ToString();
        transform.Find("NormalCrateAmt").GetComponent<Text>().text = nCrateAmt.ToString();
        transform.Find("SpecialCrateAmt").GetComponent<Text>().text = sCrateAmt.ToString();
    }
}
