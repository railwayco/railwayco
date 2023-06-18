using UnityEngine;

public class ICanScriptData : MonoBehaviour
{
    private GameLogic GameLogic { get; set; }

    void Start()
    {
        User user = new("", 0, 0, new());
        CargoMaster cargoMaster = new();
        TrainMaster trainMaster = new();
        StationMaster stationMaster = new();
        CargoCatalog cargoCatalog = new(); // Fetch from server: PlayFabClientAPI.GetTitleData
        TrainCatalog trainCatalog = new(); // Fetch from server: PlayFabClientAPI.GetTitleData

        // Setup 4 stations
        int NUM_OF_STATIONS = 4;
        for (int i = 0; i < NUM_OF_STATIONS; i++)
        {
            Station station = stationMaster.Init();
            station.Name = "Station" + (i+1).ToString();
            stationMaster.AddStation(station);
        }




        GameLogic = new(user, cargoMaster, trainMaster, stationMaster, cargoCatalog, trainCatalog);
    }
}
