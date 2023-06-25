using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class GameLogicTests
{
    // User related tests
    [Test]
    public void GameLogic_GetUserName_UserNameNotNull()
    {
        GameLogic gameLogic = GameLogicInit();
        Assert.DoesNotThrow(() => gameLogic.GetUserName());
    }

    [Test]
    public void GameLogic_GetUserExperiencePoints_ExperiencePointsNotNull()
    {
        GameLogic gameLogic = GameLogicInit();
        Assert.DoesNotThrow(() => gameLogic.GetUserExperiencePoints());
    }

    [Test]
    public void GameLogic_GetUserSkillPoints_SkillPointsNotNull()
    {
        GameLogic gameLogic = GameLogicInit();
        Assert.DoesNotThrow(() => gameLogic.GetUserSkillPoints());
    }

    [Test]
    public void GameLogic_GetUserCurrencyManager_CurrencyManagerNotNull()
    {
        GameLogic gameLogic = GameLogicInit();
        Assert.DoesNotThrow(() => gameLogic.GetUserCurrencyManager());
    }


    // CargoModel related tests
    [Test]
    public void GameLogic_GetRandomCargoModel_IsRandom()
    {
        GameLogic gameLogic = GameLogicWithCargoModelInit();

        List<CargoModel> cargoModels = new();
        int numOfCargoModels = 1000;
        for (int i = 0; i < numOfCargoModels; i++)
        {
            CargoModel cargoModel = gameLogic.GetRandomCargoModel();
            cargoModels.Add(cargoModel);
        }

        Guid guid = cargoModels[0].Guid;
        Assert.That(cargoModels.Count(c => c.Guid == guid), Is.LessThan(numOfCargoModels));
    }

    [Test]
    public void GameLogic_AddCargoModel_CargoModelIsAdded()
    {
        GameLogic gameLogic = GameLogicInit();
        CargoModel cargoModel = new(CargoType.Wood, 50.0, 100.0, new());
        gameLogic.AddCargoModel(cargoModel);
        Assert.DoesNotThrow(() => gameLogic.GetCargoModelRef(cargoModel.Guid));
    }

    [Test]
    public void GameLogic_RemoveCargoModel_CargoModelIsRemoved()
    {
        GameLogic gameLogic = GameLogicInit();
        CargoModel cargoModel = new(CargoType.Wood, 50.0, 100.0, new());
        gameLogic.AddCargoModel(cargoModel);
        gameLogic.RemoveCargoModel(cargoModel.Guid);
        Assert.Throws<NullReferenceException>(() => gameLogic.GetCargoModelRef(cargoModel.Guid));
    }

    // Station related tests
    [Test]
    public void GameLogic_InitStation_IsStationAdded()
    {
        GameLogic gameLogic = GameLogicInit();
        Guid stationGuid = gameLogic.InitStation("Station1", new());
        Assert.DoesNotThrow(() => gameLogic.GetStationRef(stationGuid));
    }

    [Test]
    public void GameLogic_AddRandomCargoToStation_StationIsNotEmpty()
    {
        GameLogic gameLogic = GameLogicWithStationsInit();
        Guid stationGuid = gameLogic.GetAllStationGuids().ToList()[0];
        int numberOfNewCargo = 10;

        Assert.IsTrue(gameLogic.GetAllCargoGuidsFromStation(stationGuid).Count == 0);
        gameLogic.AddRandomCargoToStation(stationGuid, numberOfNewCargo);
        Assert.IsTrue(gameLogic.GetAllCargoGuidsFromStation(stationGuid).Count == numberOfNewCargo);
    }

    [TestCase(StationOrientation.Head_Head, ExpectedResult = StationOrientation.Head_Head)]
    [TestCase(StationOrientation.Head_Tail, ExpectedResult = StationOrientation.Tail_Head)]
    [TestCase(StationOrientation.Tail_Tail, ExpectedResult = StationOrientation.Tail_Tail)]
    [TestCase(StationOrientation.Tail_Head, ExpectedResult = StationOrientation.Head_Tail)]
    public StationOrientation GameLogic_AddStationToStation_StationsAreLinkedCorrectly(StationOrientation orientation)
    {
        GameLogic gameLogic = GameLogicWithStationsInit();
        Guid stationGuid = gameLogic.GetAllStationGuids().ToList()[0];
        Guid newStationGuid = gameLogic.InitStation("Station3", new());
        gameLogic.AddStationToStation(stationGuid, newStationGuid, orientation);
        return gameLogic.GetStationRef(newStationGuid).StationHelper.GetObject(stationGuid);
    }
    
    [Test]
    public void GameLogic_RemoveStationFromStation_StationLinksAreRemovedCorrectly()
    {
        GameLogic gameLogic = GameLogicWithStationsInit();
        Guid station1Guid = gameLogic.GetAllStationGuids().ToList()[0];
        Guid station2Guid = gameLogic.GetAllStationGuids().ToList()[1];

        Assert.DoesNotThrow(() => gameLogic.GetStationRef(station1Guid).StationHelper.GetObject(station2Guid));
        Assert.DoesNotThrow(() => gameLogic.GetStationRef(station1Guid).StationHelper.GetObject(station2Guid));
        gameLogic.RemoveStationFromStation(station1Guid, station2Guid);
        Assert.IsTrue(gameLogic.GetStationRef(station1Guid).StationHelper.GetObject(station2Guid) == default);
        Assert.IsTrue(gameLogic.GetStationRef(station1Guid).StationHelper.GetObject(station2Guid) == default);
    }

    [TestCase(1F, 2F, 3F)]
    [TestCase(-1F, 2F, -3.5F)]
    public void GameLogic_SetStationUnityStats_StationUnityStatsCorrect(float x, float y, float z)
    {
        GameLogic gameLogic = GameLogicInit();
        Guid stationGuid = gameLogic.InitStation("Station1", new());
        Station station = gameLogic.GetStationRef(stationGuid);
        Vector3 vector = new(x, y, z);

        Assert.AreEqual(station.Attribute.Position, new Vector3());
        gameLogic.SetStationUnityStats(stationGuid, vector);

        station = gameLogic.GetStationRef(stationGuid);
        Assert.AreEqual(station.Attribute.Position, vector);
    }

    [TestCase(1F, 2F, 3F)]
    [TestCase(-1F, 2F, -3.5F)]
    public void GameLogic_GetStationRefByPosition_StationCanBeFound(float x, float y, float z)
    {
        GameLogic gameLogic = GameLogicInit();
        Vector3 vector = new(x, y, z);
        Guid stationGuid = gameLogic.InitStation("Station1", vector);

        Station station = gameLogic.GetStationRefByPosition(vector);
        Assert.AreEqual(stationGuid, station.Guid);
    }

    // Train related tests
    [Test]
    public void GameLogic_InitTrain_IsTrainAdded()
    {
        GameLogic gameLogic = GameLogicInit();
        Guid trainGuid = gameLogic.InitTrain("Train1", 5.0, new(), new(), TrainDirection.NORTH);
        Assert.DoesNotThrow(() => gameLogic.GetTrainRef(trainGuid));
    }

    [Test]
    public void GameLogic_SetTrainTravelPlan_SourceAndDestinationStationsAreCorrect()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid station1Guid = gameLogic.GetAllStationGuids().ToList()[0];
        Guid station2Guid = gameLogic.GetAllStationGuids().ToList()[1];
        Guid trainGuid = gameLogic.GetAllTrainGuids().ToList()[0];

        gameLogic.SetTrainTravelPlan(trainGuid, station1Guid, station2Guid);
        Train train = gameLogic.GetTrainRef(trainGuid);
        Assert.AreEqual(station1Guid, train.TravelPlan.SourceStation);
        Assert.AreEqual(station2Guid, train.TravelPlan.DestinationStation);
    }
    
    [Test]
    public void GameLogic_GetTrainDestination_IsTrainDestination()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid station1Guid = gameLogic.GetAllStationGuids().ToList()[0];
        Guid station2Guid = gameLogic.GetAllStationGuids().ToList()[1];
        Guid trainGuid = gameLogic.GetAllTrainGuids().ToList()[0];
        gameLogic.SetTrainTravelPlan(trainGuid, station1Guid, station2Guid);

        Train train = gameLogic.GetTrainRef(trainGuid);
        Assert.AreEqual(gameLogic.GetTrainDestination(trainGuid), train.TravelPlan.DestinationStation);
    }

    [Test]
    public void GameLogic_OnTrainArrival_IsExpectedBehaviour()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid station1Guid = gameLogic.GetAllStationGuids().ToList()[0];
        Guid station2Guid = gameLogic.GetAllStationGuids().ToList()[1];
        Guid trainGuid = gameLogic.GetAllTrainGuids().ToList()[0];
        gameLogic.SetTrainTravelPlan(trainGuid, station1Guid, station2Guid);
        Assert.IsTrue(gameLogic.GetAllCargoGuids().Count == 0);

        gameLogic.AddRandomCargoToStation(station1Guid, 10);
        Assert.IsFalse(gameLogic.GetAllCargoGuids().Count == 0);

        HashSet<Guid> guids = gameLogic.GetAllCargoGuidsFromStation(station1Guid);
        Assert.IsTrue(gameLogic.GetAllCargoGuidsFromTrain(trainGuid).Count == 0);
        
        foreach (var guid in guids) gameLogic.MoveCargoFromStationToTrain(guid, station1Guid, trainGuid);
        Assert.IsFalse(gameLogic.GetAllCargoGuidsFromTrain(trainGuid).Count == 0);

        Assert.IsTrue(gameLogic.GetStationRef(station2Guid).TrainHelper.GetAll().Count == 0);
        gameLogic.OnTrainArrival(trainGuid);
        Assert.IsTrue(gameLogic.GetAllCargoGuidsFromTrain(trainGuid).Count == 0);
        Assert.IsTrue(gameLogic.GetAllCargoGuids().Count == 0);
        Assert.IsTrue(gameLogic.GetStationRef(station2Guid).TrainHelper.GetAll().Count == 1);
    }
    
    [Test]
    public void GameLogic_OnTrainDeparture_IsExpectedBehaviour()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid station1Guid = gameLogic.GetAllStationGuids().ToList()[0];
        Guid station2Guid = gameLogic.GetAllStationGuids().ToList()[1];
        Guid trainGuid = gameLogic.GetAllTrainGuids().ToList()[0];
        gameLogic.SetTrainTravelPlan(trainGuid, station2Guid, station1Guid);
        gameLogic.OnTrainArrival(trainGuid);
        Assert.IsTrue(gameLogic.GetStationRef(station1Guid).TrainHelper.GetAll().Count == 1);

        gameLogic.OnTrainDeparture(trainGuid, station1Guid, station2Guid);
        Assert.IsTrue(gameLogic.GetTrainDestination(trainGuid) == station2Guid);
        Assert.IsTrue(gameLogic.GetStationRef(station1Guid).TrainHelper.GetAll().Count == 0);
    }

    [TestCase(1F, 2F, 3F, 4F)]
    [TestCase(-1F, 2.3F, -3.5F, 5F)]
    public void GameLogic_SetTrainUnityStats_TrainUnityStatsCorrect(float x, float y, float z, float w)
    {
        GameLogic gameLogic = GameLogicInit();
        Guid trainGuid = gameLogic.InitTrain("Train1", 5.0, new(), new(), TrainDirection.NORTH);
        Train train = gameLogic.GetTrainRef(trainGuid);
        Vector3 vector = new(x, y, z);
        Quaternion quaternion = new(x, y, z, w);

        Assert.AreEqual(0.0, train.Attribute.Speed.Amount);
        Assert.AreEqual(new Vector3(), train.Attribute.Position);
        Assert.AreEqual(new Quaternion(), train.Attribute.Rotation);
        Assert.AreEqual(TrainDirection.NORTH, train.Attribute.Direction);
        gameLogic.SetTrainUnityStats(trainGuid, x, vector, quaternion, TrainDirection.SOUTH);

        train = gameLogic.GetTrainRef(trainGuid);
        Assert.AreEqual(x, train.Attribute.Speed.Amount);
        Assert.AreEqual(vector, train.Attribute.Position);
        Assert.AreEqual(quaternion, train.Attribute.Rotation);
        Assert.AreEqual(TrainDirection.SOUTH, train.Attribute.Direction);
    }

    [TestCase(1F, 2F, 3F)]
    [TestCase(-1F, 2F, -3.5F)]
    public void GameLogic_GetTrainRefByPosition_TrainCanBeFound(float x, float y, float z)
    {
        GameLogic gameLogic = GameLogicInit();
        Vector3 vector = new(x, y, z);
        Guid trainGuid = gameLogic.InitTrain("Train1", 5.0, vector, new(), TrainDirection.NORTH);

        Train train = gameLogic.GetTrainRefByPosition(vector);
        Assert.AreEqual(trainGuid, train.Guid);
    }

    // Hybrid tests
    [Test]
    public void GameLogic_MoveCargoFromStationToTrain_CargoMovedFromStationToTrain()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid stationGuid = gameLogic.GetAllStationGuids().ToList()[0];
        Guid trainGuid = gameLogic.GetAllTrainGuids().ToList()[0];
        gameLogic.AddRandomCargoToStation(stationGuid, 10);
        Guid cargoGuid = gameLogic.GetAllCargoGuidsFromStation(stationGuid).ToList()[0];

        Assert.IsTrue(gameLogic.GetAllCargoGuidsFromStation(stationGuid).Contains(cargoGuid));
        Assert.IsFalse(gameLogic.GetAllCargoGuidsFromTrain(trainGuid).Contains(cargoGuid));
        gameLogic.MoveCargoFromStationToTrain(cargoGuid, stationGuid, trainGuid);
        Assert.IsTrue(gameLogic.GetAllCargoGuidsFromTrain(trainGuid).Contains(cargoGuid));
        Assert.IsFalse(gameLogic.GetAllCargoGuidsFromStation(stationGuid).Contains(cargoGuid));
    }

    [Test]
    public void GameLogic_MoveCargoFromTrainToStation_CargoMovedFromTrainToStation()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid stationGuid = gameLogic.GetAllStationGuids().ToList()[0];
        Guid trainGuid = gameLogic.GetAllTrainGuids().ToList()[0];
        gameLogic.AddRandomCargoToStation(stationGuid, 10);
        Guid cargoGuid = gameLogic.GetAllCargoGuidsFromStation(stationGuid).ToList()[0];
        gameLogic.MoveCargoFromStationToTrain(cargoGuid, stationGuid, trainGuid);

        Assert.IsFalse(gameLogic.GetAllCargoGuidsFromStation(stationGuid).Contains(cargoGuid));
        Assert.IsTrue(gameLogic.GetAllCargoGuidsFromTrain(trainGuid).Contains(cargoGuid));
        gameLogic.MoveCargoFromTrainToStation(cargoGuid, stationGuid, trainGuid);
        Assert.IsFalse(gameLogic.GetAllCargoGuidsFromTrain(trainGuid).Contains(cargoGuid));
        Assert.IsTrue(gameLogic.GetAllCargoGuidsFromStation(stationGuid).Contains(cargoGuid));
    }


    // Initializers
    private GameLogic GameLogicInit() => new();
    private GameLogic GameLogicWithCargoModelInit()
    {
        GameLogic gameLogic = GameLogicInit();
        gameLogic.GenerateRandomData();
        return gameLogic;
    }
    private GameLogic GameLogicWithStationsInit()
    {
        GameLogic gameLogic = GameLogicWithCargoModelInit();
        Guid station1Guid = gameLogic.InitStation("Station1", new());
        Guid station2Guid = gameLogic.InitStation("Station2", new());
        gameLogic.AddStationToStation(station1Guid, station2Guid, StationOrientation.Head_Tail);
        return gameLogic;
    }
    private GameLogic GameLogicWithStationsAndTrainInit()
    {
        GameLogic gameLogic = GameLogicWithStationsInit();
        gameLogic.InitTrain("Train1", 5.0, new(), new(), TrainDirection.NORTH);
        return gameLogic;
    }
}
