using System;
using System.Collections.Generic;
using System.Linq;

public class GameLogic
{
    private HashSet<GameDataType> GameDataTypes { get; set; }

    public User User { get; private set; }
    public WorkerDictHelper<Cargo> CargoMaster { get; private set; }
    public WorkerDictHelper<Train> TrainMaster { get; private set; }
    public WorkerDictHelper<Station> StationMaster { get; private set; }
    private PlatformMaster PlatformMaster { get; set; }
    private WorkerDictHelper<CargoModel> CargoCatalog { get; set; }
    private WorkerDictHelper<TrainModel> TrainCatalog { get; set; }

    public GameLogic()
    {
        GameDataTypes = new();

        User = new("", 0, new(0), new());
        CargoMaster = new();
        TrainMaster = new();
        StationMaster = new();
        PlatformMaster = new();
        CargoCatalog = new();
        TrainCatalog = new();

#if UNITY_EDITOR
        GenerateCargoModels();
        PopulatePlatformsAndTracks();
#endif
    }

    public void SetTrainUnityStats(
        Guid train,
        float speed,
        UnityEngine.Vector3 position,
        UnityEngine.Quaternion rotation,
        DepartDirection direction)
    {
        Train trainObject = TrainMaster.GetObject(train);
        trainObject.Attribute.SetUnityStats(speed, position, rotation, direction);
    }
    public Train GetTrainRefByPosition(UnityEngine.Vector3 position)
    {
        Train train = default;
        HashSet<Guid> trains = TrainMaster.GetAll();
        foreach (var guid in trains)
        {
            Train trainObject = TrainMaster.GetRef(guid);
            if (trainObject.Attribute.Position.Equals(position))
            {
                train = trainObject;
                break;
            }
        }
        return train;
    }
    public void OnTrainArrival(Guid train)
    {
        Train trainRef = TrainMaster.GetRef(train);

        if (trainRef.TravelPlan == default) return; // when train is just initialised
        Guid station = trainRef.TravelPlan.DestinationStation;
        CompleteTrainTravelPlan(train);

        StationMaster.GetObject(station).TrainHelper.Add(train);

        HashSet<Guid> cargoCollection = trainRef.CargoHelper.GetAll();
        foreach (Guid cargo in cargoCollection)
        {
            Cargo cargoRef = CargoMaster.GetRef(cargo);
            if (!cargoRef.TravelPlan.HasArrived(station)) continue;

            User.AddCurrencyManager(cargoRef.CurrencyManager);
            RemoveCargoFromTrain(train, cargo);
            CargoMaster.Remove(cargo);
        }

        GameDataTypes.Add(GameDataType.User);
        GameDataTypes.Add(GameDataType.CargoMaster);
    }
    public DepartStatus OnTrainDeparture(Guid train)
    {
        Train trainObject = TrainMaster.GetObject(train);
        TrainAttribute trainAttribute = trainObject.Attribute;
        if (!trainAttribute.BurnFuel())
            return TrainDepartStatus.OutOfFuel;
        if (!trainAttribute.DurabilityWear())
            return TrainDepartStatus.OutOfDurability;

        if (trainObject.TravelPlan == default) return DepartStatus.Error;
        Guid sourceStation = trainObject.TravelPlan.SourceStation;

        StationMaster.GetObject(sourceStation).TrainHelper.Remove(train);

        GameDataTypes.Add(GameDataType.TrainMaster);
        GameDataTypes.Add(GameDataType.StationMaster);
        return DepartStatus.Success;
    }
    public void SetTrainTravelPlan(Guid train, Guid sourceStation, Guid destinationStation)
    {
        Train trainObject = TrainMaster.GetObject(train);
        trainObject.FileTravelPlan(sourceStation, destinationStation);

        GameDataTypes.Add(GameDataType.TrainMaster);
    }
    public void CompleteTrainTravelPlan(Guid train)
    {
        Train trainObject = TrainMaster.GetObject(train);
        trainObject.CompleteTravelPlan();

        GameDataTypes.Add(GameDataType.TrainMaster);
    }
    public void ReplenishTrainFuelAndDurability(Guid train)
    {
        TrainAttribute trainAttribute = TrainMaster.GetObject(train).Attribute;
        trainAttribute.Refuel();
        trainAttribute.DurabilityRepair();

        GameDataTypes.Add(GameDataType.TrainMaster);
    }
    public bool SpeedUpTrainRefuel(Guid train, int coinValue)
    {
        double? coinAmt = User.CurrencyManager.GetCurrency(CurrencyType.Coin);
        if (coinAmt < coinValue)
            return false;

        User.CurrencyManager.RemoveCurrency(new(CurrencyType.Coin, coinValue));
        TrainAttribute trainAttribute = TrainMaster.GetObject(train).Attribute;
        // TODO: number of times to call this depending on how much coinValue used
        trainAttribute.Refuel();

        GameDataTypes.Add(GameDataType.TrainMaster);
        GameDataTypes.Add(GameDataType.User);

        return true;
    }
    public bool SpeedUpTrainRepair(Guid train, int coinValue)
    {
        double? coinAmt = User.CurrencyManager.GetCurrency(CurrencyType.Coin);
        if (coinAmt < coinValue)
            return false;

        User.CurrencyManager.RemoveCurrency(new(CurrencyType.Coin, coinValue));
        TrainAttribute trainAttribute = TrainMaster.GetObject(train).Attribute;
        // TODO: number of times to call this depending on how much coinValue used
        trainAttribute.DurabilityRepair();

        GameDataTypes.Add(GameDataType.TrainMaster);
        GameDataTypes.Add(GameDataType.User);

        return true;
    }
    public bool AddCargoToTrain(Guid train, Guid cargo)
    {
        Train trainObject = TrainMaster.GetObject(train);
        if (trainObject.Attribute.IsCapacityFull()) return false;

        trainObject.CargoHelper.Add(cargo);
        trainObject.Attribute.AddToCapacity();

        CargoMaster.GetObject(cargo).CargoAssoc = CargoAssociation.Train;

        GameDataTypes.Add(GameDataType.TrainMaster);
        GameDataTypes.Add(GameDataType.CargoMaster);

        return true;
    }
    public void RemoveCargoFromTrain(Guid train, Guid cargo)
    {
        Train trainObject = TrainMaster.GetObject(train);
        trainObject.CargoHelper.Remove(cargo);
        trainObject.Attribute.RemoveFromCapacity();

        GameDataTypes.Add(GameDataType.TrainMaster);
    }
    public Guid InitTrain(
        string trainName,
        double maxSpeed,
        UnityEngine.Vector3 position,
        UnityEngine.Quaternion rotation,
        DepartDirection direction)
    {
        TrainAttribute attribute = new(
            new(0, 4, 0, 0),
            new(0.0, 100.0, 100.0, 5.0),
            new(0.0, 100.0, 100.0, 5.0),
            new(0.0, maxSpeed, 0.0, 0.0),
            position,
            rotation,
            direction);
        Train train = new(
            trainName,
            TrainType.Steam,
            attribute,
            new());

        TrainMaster.Add(train);

        GameDataTypes.Add(GameDataType.TrainMaster);

        return train.Guid;
    }

    public Station GetStationRefByNumber(int stationNum)
    {
        Station station = default;
        HashSet<Guid> stations = StationMaster.GetAll();
        foreach (var guid in stations)
        {
            Station stationObject = StationMaster.GetRef(guid);
            if (stationObject.Number.Equals(stationNum))
            {
                station = stationObject;
                break;
            }
        }
        return station;
    }
    public OperationalStatus GetStationStatus(Guid station) => StationMaster.GetObject(station).Status;
    public void CloseStation(Guid station) => StationMaster.GetObject(station).Close();
    public void OpenStation(Guid station) => StationMaster.GetObject(station).Open();
    public void LockStation(Guid station)
    {
        StationMaster.GetObject(station).Lock();
        StationReacher.DisconnectStation(StationMaster, station);
    }
    public bool UnlockStation(Guid station)
    {
        double coinValue = 0; // TODO: coins required to unlock station
                              // if same for all stations
                              // else need to store in backend amt for each station
        double coinAmt = User.GetCurrencyManager().GetCurrency(CurrencyType.Coin);
        if (coinAmt < coinValue)
            return false;

        StationMaster.GetObject(station).Unlock();
        StationReacher.Bfs(StationMaster, PlatformMaster);
        return true;
    }
    public void AddRandomCargoToStation(Guid station, int numOfRandomCargo)
    {
        List<Guid> subStations = StationMaster.GetObject(station).StationHelper.GetAll().ToList();
        if (subStations.Count == 0)
            return;
        Random rand = new();

        for (int i = 0; i < numOfRandomCargo; i++)
        {
            CargoModel cargoModel = GetRandomCargoModel();
            cargoModel.Randomise();
            Guid destination = subStations[rand.Next(subStations.Count - 1)];

            Cargo cargo = new(cargoModel, station, destination);
            CargoMaster.Add(cargo);
            AddCargoToStation(station, cargo.Guid);
        }

        GameDataTypes.Add(GameDataType.CargoMaster);
    }
    public bool AddCargoToStation(Guid station, Guid cargo)
    {
        Station stationObject = StationMaster.GetObject(station);
        Cargo cargoObject = CargoMaster.GetObject(cargo);

        if (!cargoObject.TravelPlan.IsAtSource(station))
        {
            if (stationObject.Attribute.IsYardFull())
                return false;
            stationObject.Attribute.AddToYard();
            cargoObject.CargoAssoc = CargoAssociation.Yard;
        }
        else
            cargoObject.CargoAssoc = CargoAssociation.Station;

        stationObject.CargoHelper.Add(cargo);

        GameDataTypes.Add(GameDataType.StationMaster);
        GameDataTypes.Add(GameDataType.CargoMaster);
        return true;
    }
    public void RemoveCargoFromStation(Guid station, Guid cargo)
    {
        Station stationObject = StationMaster.GetObject(station);
        stationObject.CargoHelper.Remove(cargo);

        Cargo cargoObject = CargoMaster.GetRef(cargo);
        if (!cargoObject.TravelPlan.IsAtSource(station))
            stationObject.Attribute.RemoveFromYard();

        GameDataTypes.Add(GameDataType.StationMaster);
    }
    public Guid InitStation(int stationNumber, UnityEngine.Vector3 position)
    {
        StationAttribute stationAttribute = new(
            new(0, 5, 0, 0));
        Station station = new(
                stationNumber,
                OperationalStatus.Open,
                stationAttribute,
                new(),
                new(),
                new());

        StationMaster.Add(station);
        StationReacher.Bfs(StationMaster, PlatformMaster);

        GameDataTypes.Add(GameDataType.StationMaster);

        return station.Guid;
    }

    public HashSet<int> GetPlatformNeighbours(Guid platform)
    {
        List<Track> tracks = PlatformMaster.GetPlatformTracks(platform).ToList();
        HashSet<int> stationNums = new();
        tracks.ForEach(track => 
        {
            int stationNum = PlatformMaster.GetPlatform(track.Platform).StationNum;
            stationNums.Add(stationNum);
        });
        return stationNums;
    }
    public OperationalStatus GetTrackStatus(Guid platform1, Guid platform2)
    {
        return PlatformMaster.GetPlatformTrack(platform1, platform2).Status;
    }
    public OperationalStatus GetPlatformStatus(Guid platform)
    {
        return PlatformMaster.GetPlatform(platform).Status;
    }
    public Guid GetPlatformGuid(int stationNum, int platformNum)
    {
        return PlatformMaster.GetPlatformGuidByStationAndPlatformNum(stationNum, platformNum);
    }

    public CargoModel GetRandomCargoModel()
    {
        List<Guid> keys = CargoCatalog.GetAll().ToList();

        Random rand = new();
        int randomIndex = rand.Next(keys.Count - 1);

        Guid randomGuid = keys[randomIndex];
        return CargoCatalog.GetRef(randomGuid);
    }

    public void SetDataFromPlayfab(GameDataType gameDataType, string data)
    {
        switch (gameDataType)
        {
            case GameDataType.User:
                User = GameDataManager.Deserialize<User>(data);
                break;
            case GameDataType.CargoMaster:
                CargoMaster = GameDataManager.Deserialize<WorkerDictHelper<Cargo>>(data);
                break;
            case GameDataType.CargoCatalog:
                CargoCatalog = GameDataManager.Deserialize<WorkerDictHelper<CargoModel>>(data);
                break;
            case GameDataType.TrainMaster:
                TrainMaster = GameDataManager.Deserialize<WorkerDictHelper<Train>>(data);
                break;
            case GameDataType.TrainCatalog:
                TrainCatalog = GameDataManager.Deserialize<WorkerDictHelper<TrainModel>>(data);
                break;
            case GameDataType.StationMaster:
                StationMaster = GameDataManager.Deserialize<WorkerDictHelper<Station>>(data);
                break;
            case GameDataType.PlatformMaster:
                PlatformMaster = GameDataManager.Deserialize<PlatformMaster>(data);
                break;
            default:
                break;
        }
    }
    public void SendDataToPlayfab()
    {
#if UNITY_EDITOR
#else
        Dictionary<GameDataType, string> gameDataDict = new();
        List<GameDataType> gameDataTypes = GameDataTypes.ToList();
        if (gameDataTypes.Count == 0) return;

        gameDataTypes.ForEach(gameDataType => 
        {
            string data = gameDataType switch
            {
                GameDataType.User => GameDataManager.Serialize(User),
                GameDataType.CargoMaster => GameDataManager.Serialize(CargoMaster),
                GameDataType.CargoCatalog => GameDataManager.Serialize(CargoCatalog),
                GameDataType.TrainMaster => GameDataManager.Serialize(TrainMaster),
                GameDataType.TrainCatalog => GameDataManager.Serialize(TrainCatalog),
                GameDataType.StationMaster => GameDataManager.Serialize(StationMaster),
                GameDataType.PlatformMaster => GameDataManager.Serialize(PlatformMaster),
                _ => throw new NotImplementedException()
            };
            gameDataDict.Add(gameDataType, data);
        });
        GameDataTypes = new();
        GameDataManager.UpdateUserData(gameDataDict);
#endif
    }


    /////////// Data Generation ///////////  
    public void GenerateCargoModels()
    {
        CargoType[] cargoTypes = (CargoType[])Enum.GetValues(typeof(CargoType));
        CurrencyType[] currencyTypes = (CurrencyType[])Enum.GetValues(typeof(CurrencyType));
        foreach (var cargoType in cargoTypes)
        {
            Random rand = new(Guid.NewGuid().GetHashCode());
            CurrencyManager currencyManager = new();
            CurrencyType randomType = currencyTypes[rand.Next(currencyTypes.Length)];
            int randomAmount = randomType switch
            {
                CurrencyType.Coin => rand.Next(10, 100),
                CurrencyType.Note => rand.Next(1, 5),
                CurrencyType.NormalCrate => rand.Next(1, 1),
                CurrencyType.SpecialCrate => rand.Next(1, 1),
                _ => rand.Next(1, 1),
            };
            Currency currency = new(randomType, randomAmount);
            currencyManager.AddCurrency(currency);

            CargoModel cargoModel = new(cargoType, 15, 20, currencyManager);
            CargoCatalog.Add(cargoModel);
        }

        GameDataTypes.Add(GameDataType.CargoCatalog);
    }

    public void PopulatePlatformsAndTracks()
    {
        // Create each platform in PlatformMaster
        // 7 stations, 2 platforms each
        for (int i = 1; i <= 7; i++)
        {
            for (int j = 1; j <= 2; j++)
            {
                Platform platform = new(i, j);
                PlatformMaster.AddPlatform(platform);
            }
        }

        // Link all the different platforms together using Track
        Guid platform_1_1 = PlatformMaster.GetPlatformGuidByStationAndPlatformNum(1, 1);
        Guid platform_2_1 = PlatformMaster.GetPlatformGuidByStationAndPlatformNum(2, 1);
        Guid platform_6_1 = PlatformMaster.GetPlatformGuidByStationAndPlatformNum(6, 1);
        PlatformMaster.AddPlatformTrack(platform_1_1, platform_2_1);
        PlatformMaster.AddPlatformTrack(platform_1_1, platform_6_1);
        PlatformMaster.UnlockPlatform(platform_1_1);
        PlatformMaster.UnlockPlatform(platform_2_1);
        PlatformMaster.UnlockPlatform(platform_6_1);
        PlatformMaster.UnlockPlatformTrack(platform_1_1, platform_2_1);
        PlatformMaster.UnlockPlatformTrack(platform_1_1, platform_6_1);

        Guid platform_2_2 = PlatformMaster.GetPlatformGuidByStationAndPlatformNum(2, 2);
        Guid platform_7_2 = PlatformMaster.GetPlatformGuidByStationAndPlatformNum(7, 2);
        Guid platform_3_1 = PlatformMaster.GetPlatformGuidByStationAndPlatformNum(3, 1);
        PlatformMaster.AddPlatformTrack(platform_2_2, platform_7_2);
        PlatformMaster.AddPlatformTrack(platform_2_2, platform_3_1);
        PlatformMaster.UnlockPlatform(platform_2_2);
        PlatformMaster.UnlockPlatform(platform_7_2);
        PlatformMaster.UnlockPlatform(platform_3_1);
        PlatformMaster.UnlockPlatformTrack(platform_2_2, platform_7_2);
        PlatformMaster.UnlockPlatformTrack(platform_2_2, platform_3_1);

        Guid platform_6_2 = PlatformMaster.GetPlatformGuidByStationAndPlatformNum(6, 2);
        Guid platform_7_1 = PlatformMaster.GetPlatformGuidByStationAndPlatformNum(7, 1);
        PlatformMaster.AddPlatformTrack(platform_6_2, platform_7_1);

        Guid platform_3_2 = PlatformMaster.GetPlatformGuidByStationAndPlatformNum(3, 2);
        Guid platform_4_2 = PlatformMaster.GetPlatformGuidByStationAndPlatformNum(4, 2);
        Guid platform_5_2 = PlatformMaster.GetPlatformGuidByStationAndPlatformNum(5, 2);
        PlatformMaster.AddPlatformTrack(platform_3_2, platform_4_2);
        PlatformMaster.AddPlatformTrack(platform_4_2, platform_5_2);

        Guid platform_1_2 = PlatformMaster.GetPlatformGuidByStationAndPlatformNum(1, 2);
        Guid platform_4_1 = PlatformMaster.GetPlatformGuidByStationAndPlatformNum(4, 1);
        Guid platform_5_1 = PlatformMaster.GetPlatformGuidByStationAndPlatformNum(5, 1);
        PlatformMaster.AddPlatformTrack(platform_1_2, platform_4_1);
        PlatformMaster.AddPlatformTrack(platform_4_1, platform_5_1);
        PlatformMaster.UnlockPlatform(platform_1_2);
        PlatformMaster.UnlockPlatform(platform_4_1);
        PlatformMaster.UnlockPlatformTrack(platform_1_2, platform_4_1);

        GameDataTypes.Add(GameDataType.PlatformMaster);
    }
}
