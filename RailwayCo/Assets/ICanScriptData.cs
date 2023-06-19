using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Text;
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

        // Setup 1 type of train in TrainMaster
        TrainAttribute attribute = new(
            new(0, 4, 0, 0),
            new(0.0, 100.0, 100.0, 5.0),
            new(0.0, 100.0, 100.0, 5.0),
            new(0.0, 200.0, 0.0, 0.0));
        Train train = trainMaster.Init("Train1", TrainType.Steam, attribute, new());
        trainMaster.AddTrain(train);

        // Setup a type of cargo in CargoCatalog
        CurrencyManager currencyManager = new();
        Currency currency = new(CurrencyType.Coin, 500);
        currencyManager.AddCurrency(currency);
        CargoModel cargoModel = cargoCatalog.Init(CargoType.Wood, 15, 20, currencyManager);
        cargoCatalog.AddCargoModel(cargoModel);

        GameLogic = new(user, cargoMaster, trainMaster, stationMaster, cargoCatalog, trainCatalog);
        GameLogic.GenerateNewCargo(10);

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings();
        JsonSerializer serializer = new JsonSerializer();
        serializer.Converters.Add(new StringEnumConverter());

        using (StreamWriter sw = new StreamWriter(@"Assets/GameData.json"))
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            serializer.Serialize(writer, GameLogic);
        }


        GameLogic newGameLogic;
        using (StreamReader sr = new StreamReader(@"Assets/GameData.json"))
        using (JsonReader reader = new JsonTextReader(sr))
        {
            newGameLogic = serializer.Deserialize<GameLogic>(reader);
        }

        Debug.Log(newGameLogic.GetAllCargo());
    }
}
