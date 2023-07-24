using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RailwayCoSO", menuName = "RailwayCo/GameLogic")]
public class GameLogic : ScriptableObject
{
    private User User { get; set; }
    private CargoMaster CargoMaster { get; set; }
    private TrainMaster TrainMaster { get; set; }
    private StationMaster StationMaster { get; set; }
    private PlatformMaster PlatformMaster { get; set; }


#if UNITY_EDITOR
    private void OnEnable()
    {
        // Source: https://forum.unity.com/threads/solved-but-unhappy-scriptableobject-awake-never-execute.488468/#post-5564170
        // use platform dependent compilation so it only exists in editor, otherwise it'll break the build
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            Init();
    }
#endif

    private void Awake() => Init();

    private void Init()
    {
        User = new("", 0, new(0), new());
        CargoMaster = new();
        TrainMaster = new();
        StationMaster = new();
        PlatformMaster = new();

        PlatformMaster.InitPlatformsAndTracks();
    }


    #region User Related Methods
    public int GetUserExperiencePoints() => User.ExperiencePoint;
    public CurrencyManager GetUserCurrencyManager() => User.GetCurrencyManager();
#if UNITY_EDITOR
    public void AddUserCurrencyManager(CurrencyManager currencyManager) => User.AddCurrencyManager(currencyManager);
#endif
    public bool RemoveUserCurrencyManager(CurrencyManager currencyManager)
    {
        CurrencyManager userCurrencyManager = User.GetCurrencyManager();
        List<CurrencyType> currencyTypes = currencyManager.CurrencyTypes;
        foreach (var currencyType in currencyTypes)
        {
            int userCurrencyVal = userCurrencyManager.GetCurrency(currencyType);
            int costCurrencyVal = currencyManager.GetCurrency(currencyType);
            if (costCurrencyVal > userCurrencyVal)
                return false;
        }
        User.RemoveCurrencyManager(currencyManager);
        return true;
    }
    #endregion


    #region Cargo Related Methods
    public Cargo GetCargoObject(Guid cargo) => CargoMaster.GetObject(cargo);
    #endregion


    #region Train Related Methods
    public HashSet<Guid> GetAllTrainGuids() => TrainMaster.GetAllGuids();
    public Train GetTrainObject(Vector3 position) => TrainMaster.GetObject(position);
    public Train GetTrainObject(Guid trainGuid) => TrainMaster.GetObject(trainGuid);
    public Guid AddTrainObject(
        TrainType trainType,
        double maxSpeed,
        Vector3 position,
        Quaternion rotation,
        MovementDirection movementDirection)
    {
        return TrainMaster.AddObject(trainType, maxSpeed, position, rotation, movementDirection);
    }
    public void OnTrainArrival(Guid train)
    {
        Guid destStation = TrainMaster.GetArrivalStation(train);
        if (destStation == default) return; // when train is just initialised
        TrainMaster.CompleteTravelPlan(train);
        TrainMaster.FileTravelPlan(train, destStation, default);
        StationMaster.AddTrainToStation(destStation, train);

        HashSet<Guid> cargoCollection = TrainMaster.GetCargoManifest(train);
        foreach (Guid cargo in cargoCollection)
        {
            if (!CargoMaster.HasCargoArrived(cargo, destStation)) continue;

            CurrencyManager cargoCurrencyManager = CargoMaster.GetCurrencyManager(cargo);
            User.AddCurrencyManager(cargoCurrencyManager);

            RemoveCargoFromTrain(train, cargo);
            CargoMaster.RemoveObject(cargo);
        }

        GenerateNewCargoForStation(destStation);
    }
    public DepartStatus OnTrainDeparture(Guid train)
    {
        if (!TrainMaster.BurnFuel(train))
            return DepartStatus.OutOfFuel;
        if (!TrainMaster.Wear(train))
            return DepartStatus.OutOfDurability;

        Guid sourceStation = TrainMaster.GetDepartureStation(train);
        if (sourceStation == default)
            return DepartStatus.Error;

        StationMaster.RemoveTrainFromStation(sourceStation, train);
        return DepartStatus.Success;
    }
    public void OnTrainCollision(Guid train)
    {
        HashSet<Guid> cargoCollection = TrainMaster.GetCargoManifest(train);
        foreach (Guid cargo in cargoCollection)
        {
            CargoMaster.RemoveObject(cargo);
        }

        TrainMaster.RemoveObject(train);
    }
    public void OnTrainRemoval(Guid train)
    {
        Guid station = TrainMaster.GetDepartureStation(train);
        StationMaster.RemoveTrainFromStation(station, train);
        TrainMaster.DeactivateTrain(train);

        HashSet<Guid> cargoCollection = TrainMaster.GetCargoManifest(train);
        foreach (Guid cargo in cargoCollection)
        {
            RemoveCargoFromTrain(train, cargo);
            CargoMaster.RemoveObject(cargo);
        }
    }
    public void OnTrainRestoration(Guid train, Guid station)
    {
        TrainMaster.ActivateTrain(train);
        if (station == default) return;

        TrainMaster.FileTravelPlan(train, station, default);
        StationMaster.AddTrainToStation(station, train);
        GenerateNewCargoForStation(station);
    }
    public void ReplenishTrainFuelAndDurability(Guid train)
    {
        TrainMaster.Refuel(train);
        TrainMaster.Repair(train);
    }
    public bool SpeedUpTrainRefuel(Guid train, CurrencyManager cost)
    {
        if (!RemoveUserCurrencyManager(cost))
            return false;
        TrainMaster.Refuel(train);
        return true;
    }
    public bool SpeedUpTrainRepair(Guid train, CurrencyManager cost)
    {
        if (!RemoveUserCurrencyManager(cost))
            return false;
        TrainMaster.Repair(train);
        return true;
    }
    public void SetTrainUnityStats(
        Guid train,
        float speed,
        Vector3 position,
        Quaternion rotation,
        MovementDirection movementDirection)
    {
        TrainMaster.SetUnityStats(train, speed, position, rotation, movementDirection);
    }
    public TrainAttribute GetTrainAttribute(Guid train) => TrainMaster.GetTrainAttribute(train);
    public bool AddCargoToTrain(Guid train, Guid cargo)
    {
        if (!TrainMaster.AddCargoToTrain(train, cargo))
            return false;
        CargoMaster.SetCargoAssociation(cargo, CargoAssociation.Train);
        return true;
    }
    public void RemoveCargoFromTrain(Guid train, Guid cargo)
    {
        TrainMaster.RemoveCargoFromTrain(train, cargo);
    }
    public void SetTrainTravelPlan(Guid train, Guid sourceStation, Guid destinationStation)
    {
        TrainMaster.FileTravelPlan(train, sourceStation, destinationStation);
    }
    public Guid GetTrainDepartureStation(Guid train) => TrainMaster.GetDepartureStation(train);
    #endregion


    #region Station Related Methods
    public Station GetStationObject(int stationNum) => StationMaster.GetObject(stationNum);
    public Station GetStationObject(Guid stationGuid) => StationMaster.GetObject(stationGuid);
    public Guid AddStationObject(int stationNum)
    {
        Guid station = StationMaster.AddObject(stationNum);
        StationReacher.Bfs(StationMaster, PlatformMaster);
        return station;
    }
    /// <summary>
    /// Add a number of cargo to station as specified
    /// </summary>
    /// <param name="station">Guid of station</param>
    /// <param name="numRandomCargo">Number of random cargo to add</param>
    public void AddRandomCargoToStation(Guid station, int numRandomCargo)
    {
        IEnumerable<CargoModel> cargoModels = CargoMaster.GetRandomCargoModels(numRandomCargo);
        IEnumerator<Guid> destinations = StationMaster.GetRandomDestinations(station, numRandomCargo);
        if (!destinations.MoveNext())
            return;
        foreach (CargoModel cargoModel in cargoModels)
        {
            Guid destination = destinations.Current;
            Guid cargo = CargoMaster.AddObject(cargoModel, station, destination);
            AddCargoToStation(station, cargo);
            destinations.MoveNext();
        }
    }
    public bool AddCargoToStation(Guid station, Guid cargo)
    {
        if (!CargoMaster.IsCargoAtSource(cargo, station))
        {
            if (!StationMaster.AddCargoToYard(station, cargo))
                return false;
            CargoMaster.SetCargoAssociation(cargo, CargoAssociation.Yard);
        }
        else
        {
            StationMaster.AddCargoToStation(station, cargo);
            CargoMaster.SetCargoAssociation(cargo, CargoAssociation.Station);
        }
        return true;
    }
    public void RemoveCargoFromStation(Guid station, Guid cargo)
    {
        if (!CargoMaster.IsCargoAtSource(cargo, station))
            StationMaster.RemoveCargoFromYard(station, cargo);
        else
            StationMaster.RemoveCargoFromStation(station, cargo);
    }
    public HashSet<Guid> GetStationCargoManifest(Guid station)
    {
        return StationMaster.GetStationCargoManifest(station);
    }
    public HashSet<Guid> GetYardCargoManifest(Guid station)
    {
        return StationMaster.GetYardCargoManifest(station);
    }
    /// <summary>
    /// Generates a new set of cargo and replaces old cargo in station
    /// </summary>
    /// <param name="station">Guid of station</param>
    public void GenerateNewCargoForStation(Guid station)
    {
        int numStationCargoMax = 10;

        HashSet<Guid> manifest = StationMaster.GetStationCargoManifest(station);
        if (numStationCargoMax - manifest.Count == 0)
            return;

        foreach (Guid cargo in manifest)
        {
            StationMaster.RemoveCargoFromStation(station, cargo);
            CargoMaster.RemoveObject(cargo);
        }
        AddRandomCargoToStation(station, numStationCargoMax);
    }
    #endregion


    #region Platform Related Methods
    public HashSet<int> GetPlatformNeighbours(Guid platform)
    {
        return PlatformMaster.GetPlatformNeighbours(platform);
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
    public bool UnlockTrack(Guid source, Guid destination, CurrencyManager cost)
    {
        if (!RemoveUserCurrencyManager(cost))
            return false;
        PlatformMaster.UnlockPlatformTrack(source, destination);
        StationReacher.Bfs(StationMaster, PlatformMaster);
        return true;
    }
    public bool UnlockPlatform(Guid platform, CurrencyManager cost)
    {
        if (!RemoveUserCurrencyManager(cost))
            return false;
        PlatformMaster.UnlockPlatform(platform);
        StationReacher.Bfs(StationMaster, PlatformMaster);
        return true;
    }
    #endregion


    #region PlayFab Related Methods
    public void SetDataFromPlayfab(GameDataType gameDataType, string data)
    {
        switch (gameDataType)
        {
            case GameDataType.User:
                User = GameDataManager.Deserialize<User>(data);
                break;
            case GameDataType.CargoMaster:
                CargoMaster.SetDataFromPlayfab(data);
                break;
            case GameDataType.TrainMaster:
                TrainMaster.SetDataFromPlayfab(data);
                break;
            case GameDataType.StationMaster:
                StationMaster.SetDataFromPlayfab(data);
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
#if !UNITY_EDITOR
        Dictionary<GameDataType, string> gameDataDict = new();
        List<GameDataType> gameDataTypes = new((GameDataType[])Enum.GetValues(typeof(GameDataType)));
        gameDataTypes.ForEach(gameDataType => 
        {
            string data = gameDataType switch
            {
                GameDataType.User => GameDataManager.Serialize(User),
                GameDataType.CargoMaster => CargoMaster.SendDataToPlayfab(),
                GameDataType.TrainMaster => TrainMaster.SendDataToPlayfab(),
                GameDataType.StationMaster => StationMaster.SendDataToPlayfab(),
                GameDataType.PlatformMaster => GameDataManager.Serialize(PlatformMaster),
                _ => throw new NotImplementedException()
            };
            gameDataDict.Add(gameDataType, data);
        });
        GameDataManager.UpdateUserData(gameDataDict);
#endif
    }
    #endregion
}
