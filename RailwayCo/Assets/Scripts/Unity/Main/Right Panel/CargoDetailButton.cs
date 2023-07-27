using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CargoDetailButton : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] private Button _cargoDetailButton;
    private CargoPanelManager _cargoPanelMgr;
    private Cargo _cargo;

    // Setup for the Cargo detail button
    public void SetCargoInformation(Cargo cargo, int stationNum, bool disableButton) 
    {
        _cargo = cargo;
        PopulateCargoInformation(stationNum);
        this.GetComponent<Button>().enabled = !disableButton;
    }

    /////////////////////////////////////////////////////
    // INITIALISATION
    /////////////////////////////////////////////////////
    private void Awake()
    {
        if (!_cargoDetailButton) Debug.LogError("Cargo Detail button did not reference itself");
        _cargoDetailButton.onClick.AddListener(OnButtonClicked);

        GameObject rightPanel = GameObject.FindGameObjectWithTag("MainUI").transform.Find("RightPanel").gameObject;
        _cargoPanelMgr = rightPanel.GetComponentInChildren<CargoPanelManager>(true);
        if (!_cargoPanelMgr) Debug.LogError("CargoPanelManager not found");
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
            Destroy(this.gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Hide();
    }

    private void PopulateCargoInformation(int stationNum)
    {
        string dest = $"Station {stationNum}";
        string cargoType = _cargo.Type.ToString();
        string weight = _cargo.Weight.ToString();
        string cargoDetail = $"{cargoType} ({weight} t)";

        CurrencyManager currMgr = _cargo.CurrencyManager;
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
