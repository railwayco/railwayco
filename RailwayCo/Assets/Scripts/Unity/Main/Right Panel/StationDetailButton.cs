using UnityEngine;
using UnityEngine.UI;

public class StationDetailButton : MonoBehaviour
{
    [SerializeField] private Button _stationButton;
    private GameObject _stationToFollow;

    // Populate the station button object with the relevant information
    public void SetStationGameObject(GameObject station)
    {
        _stationToFollow = station;
        this.transform.Find("IconRectangle").GetComponent<Image>().sprite = station.GetComponent<SpriteRenderer>().sprite;
        this.transform.Find("StationName").GetComponent<Text>().text = station.name;
    }

    private void Awake()
    {
        if (!_stationButton) Debug.LogError("Station Detail Button not attached");
        _stationButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        _stationToFollow.GetComponent<StationManager>().LoadCargoPanelViaStation();
        _stationToFollow.GetComponent<StationManager>().followStation();
    }
}
