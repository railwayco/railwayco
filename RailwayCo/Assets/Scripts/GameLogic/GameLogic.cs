using System;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic
{
    private User User { get; set; }
    private CargoMaster CargoMaster { get; set; }
    private TrainMaster TrainMaster { get; set; }
    private StationMaster StationMaster { get; set; }
    private PlatformMaster PlatformMaster { get; set; }

    public GameLogic()
    {
        User = new("", 0, new(0), new());
        CargoMaster = new();
        TrainMaster = new();
        StationMaster = new();
        PlatformMaster = new();
    }


    #region User Related Methods
    public int GetUserExperiencePoints() => User.ExperiencePoint;
    public CurrencyManager GetUserCurrencyManager() => User.GetCurrencyManager();
#if UNITY_EDITOR
    public void AddUserCurrencyManager(CurrencyManager currencyManager) => User.AddCurrencyManager(currencyManager);
#endif
    #endregion


    #region Cargo Related Methods
    public Cargo GetCargoObject(Guid cargo) => CargoMaster.GetObject(cargo);
    #endregion


    #region Train Related Methods
    public Train GetTrainObject(Vector3 position) => TrainMaster.GetObject(position);
    public Train GetTrainObject(Guid trainGuid) => TrainMaster.GetObject(trainGuid);
    public Guid AddTrainObject(
        string trainName,
        double maxSpeed,
        Vector3 position,
        Quaternion rotation,
        DepartDirection direction)
    {
        return TrainMaster.AddObject(trainName, maxSpeed, position, rotation, direction);
    }
    public void OnTrainArrival(Guid train)
    {
        Guid destStation = TrainMaster.GetArrivalStation(train);
        if (destStation == default) return; // when train is just initialised
        TrainMaster.CompleteTravelPlan(train);
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
    public void ReplenishTrainFuelAndDurability(Guid train)
    {
        TrainMaster.Refuel(train);
        TrainMaster.Repair(train);
    }
    public bool SpeedUpTrainRefuel(Guid train, int coinValue)
    {
        double coinAmt = User.GetCurrency(CurrencyType.Coin);
        if (coinAmt < coinValue)
            return false;

        User.RemoveCurrency(CurrencyType.Coin, coinValue);
        // TODO: number of times to call this depending on how much coinValue used
        TrainMaster.Refuel(train);
        return true;
    }
    public bool SpeedUpTrainRepair(Guid train, int coinValue)
    {
        double coinAmt = User.GetCurrencyManager().GetCurrency(CurrencyType.Coin);
        if (coinAmt < coinValue)
            return false;

        User.RemoveCurrency(CurrencyType.Coin, coinValue);
        // TODO: number of times to call this depending on how much coinValue used
        TrainMaster.Repair(train);
        return true;
    }
    public void SetTrainUnityStats(
        Guid train,
        float speed,
        Vector3 position,
        Quaternion rotation,
        DepartDirection direction)
    {
        TrainMaster.SetUnityStats(train, speed, position, rotation, direction);
    }
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
        TrainMaster.SetTravelPlan(train, sourceStation, destinationStation);
    }
    #endregion


    #region Station Related Methods
    public Station GetStationObject(int stationNum) => StationMaster.GetObject(stationNum);
    public Station GetStationObject(Guid stationGuid) => StationMaster.GetObject(stationGuid);
    public Guid AddStationObject(int stationNum) => StationMaster.AddObject(stationNum, PlatformMaster);
    public OperationalStatus GetStationStatus(Guid station) => StationMaster.GetStationStatus(station);
    public void CloseStation(Guid station) => StationMaster.CloseStation(station);
    public void OpenStation(Guid station) => StationMaster.OpenStation(station);
    public void LockStation(Guid station) => StationMaster.LockStation(station);
    public bool UnlockStation(Guid station)
    {
        int coinValue = 0; // TODO: coins required to unlock station
                              // if same for all stations
                              // else need to store in backend amt for each station
        int coinAmt = User.GetCurrencyManager().GetCurrency(CurrencyType.Coin);
        if (coinAmt < coinValue)
            return false;

        StationMaster.UnlockStation(station, PlatformMaster);
        return true;
    }
    public void AddRandomCargoToStation(Guid station, int numRandomCargo)
    {
        IEnumerable<CargoModel> cargoModels = CargoMaster.GetRandomCargoModels(numRandomCargo);
        IEnumerator<Guid> destinations = StationMaster.GetRandomDestinations(station, numRandomCargo);
        destinations.MoveNext();
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

    public void UnlockNewTrack(Guid src, Guid dst)
    {
        PlatformMaster.UnlockPlatformTrack(src, dst);
    }

    public void UnlockNewPlatform(Guid platform)
    {
        PlatformMaster.UnlockPlatform(platform);
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
                CargoMaster = GameDataManager.Deserialize<CargoMaster>(data);
                break;
            case GameDataType.TrainMaster:
                TrainMaster = GameDataManager.Deserialize<TrainMaster>(data);
                break;
            case GameDataType.StationMaster:
                StationMaster = GameDataManager.Deserialize<StationMaster>(data);
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
                GameDataType.TrainMaster => GameDataManager.Serialize(TrainMaster),
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
    #endregion
}
