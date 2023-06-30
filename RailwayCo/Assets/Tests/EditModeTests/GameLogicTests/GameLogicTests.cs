using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class GameLogicTests
{
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

    // Station related tests
    [Test]
    public void GameLogic_InitStation_IsStationAdded()
    {
        GameLogic gameLogic = GameLogicInit();
        Guid stationGuid = gameLogic.InitStation("Station1", new());
        Assert.DoesNotThrow(() => gameLogic.StationMaster.GetRef(stationGuid));
    }

    [Test]
    public void GameLogic_AddRandomCargoToStation_StationIsNotEmpty()
    {
        GameLogic gameLogic = GameLogicWithStationsInit();
        Guid stationGuid = gameLogic.StationMaster.GetAll().ToList()[0];
        int numberOfNewCargo = 10;

        Assert.IsTrue(gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().Count == 0);
        gameLogic.AddRandomCargoToStation(stationGuid, numberOfNewCargo);
        Assert.IsTrue(gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().Count == numberOfNewCargo);
    }

    [TestCase(StationOrientation.Head_Head, ExpectedResult = StationOrientation.Head_Head)]
    [TestCase(StationOrientation.Head_Tail, ExpectedResult = StationOrientation.Tail_Head)]
    [TestCase(StationOrientation.Tail_Tail, ExpectedResult = StationOrientation.Tail_Tail)]
    [TestCase(StationOrientation.Tail_Head, ExpectedResult = StationOrientation.Head_Tail)]
    public StationOrientation GameLogic_AddTrack_StationsAreLinkedCorrectly(StationOrientation orientation)
    {
        GameLogic gameLogic = GameLogicWithStationsInit();
        Guid stationGuid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid newStationGuid = gameLogic.InitStation("Station3", new());
        gameLogic.AddTrack(stationGuid, newStationGuid, orientation);
        return gameLogic.StationMaster.GetRef(newStationGuid).StationHelper.GetObject(stationGuid);
    }
    
    [Test]
    public void GameLogic_RemoveTrack_StationLinksAreRemovedCorrectly()
    {
        GameLogic gameLogic = GameLogicWithStationsInit();
        Guid station1Guid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid station2Guid = gameLogic.StationMaster.GetAll().ToList()[1];

        Assert.DoesNotThrow(() => gameLogic.StationMaster.GetRef(station1Guid).StationHelper.GetObject(station2Guid));
        Assert.DoesNotThrow(() => gameLogic.StationMaster.GetRef(station1Guid).StationHelper.GetObject(station2Guid));
        gameLogic.RemoveTrack(station1Guid, station2Guid);
        Assert.IsTrue(gameLogic.StationMaster.GetRef(station1Guid).StationHelper.GetObject(station2Guid) == default);
        Assert.IsTrue(gameLogic.StationMaster.GetRef(station1Guid).StationHelper.GetObject(station2Guid) == default);
    }

    [TestCase(1F, 2F, 3F)]
    [TestCase(-1F, 2F, -3.5F)]
    public void GameLogic_SetStationUnityStats_StationUnityStatsCorrect(float x, float y, float z)
    {
        GameLogic gameLogic = GameLogicInit();
        Guid stationGuid = gameLogic.InitStation("Station1", new());
        Station station = gameLogic.StationMaster.GetObject(stationGuid);
        Vector3 vector = new(x, y, z);

        Assert.AreEqual(station.Attribute.Position, new Vector3());
        station.Attribute.SetUnityStats(vector);

        station = gameLogic.StationMaster.GetRef(stationGuid);
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

    [Test]
    public void GameLogic_AddCargoToStation_CargoAddedToStation()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid stationGuid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid trainGuid = gameLogic.TrainMaster.GetAll().ToList()[0];
        gameLogic.AddRandomCargoToStation(stationGuid, 10);
        Guid cargoGuid = gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().ToList()[0];
        gameLogic.RemoveCargoFromStation(stationGuid, cargoGuid);
        gameLogic.AddCargoToTrain(trainGuid, cargoGuid);

        Assert.IsFalse(gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().Contains(cargoGuid));
        gameLogic.AddCargoToStation(stationGuid, cargoGuid);
        Assert.IsTrue(gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().Contains(cargoGuid));
    }

    [Test]
    public void GameLogic_RemoveCargoFromStation_CargoRemovedFromStation()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid stationGuid = gameLogic.StationMaster.GetAll().ToList()[0];
        gameLogic.AddRandomCargoToStation(stationGuid, 10);
        Guid cargoGuid = gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().ToList()[0];

        Assert.IsTrue(gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().Contains(cargoGuid));
        gameLogic.RemoveCargoFromStation(stationGuid, cargoGuid);
        Assert.IsFalse(gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().Contains(cargoGuid));
    }

    // Train related tests
    [Test]
    public void GameLogic_InitTrain_IsTrainAdded()
    {
        GameLogic gameLogic = GameLogicInit();
        Guid trainGuid = gameLogic.InitTrain("Train1", 5.0, new(), new(), TrainDirection.NORTH);
        Assert.DoesNotThrow(() => gameLogic.TrainMaster.GetRef(trainGuid));
    }

    [Test]
    public void GameLogic_SetTrainTravelPlan_SourceAndDestinationStationsAreCorrect()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid station1Guid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid station2Guid = gameLogic.StationMaster.GetAll().ToList()[1];
        Guid trainGuid = gameLogic.TrainMaster.GetAll().ToList()[0];

        gameLogic.SetTrainTravelPlan(trainGuid, station1Guid, station2Guid);
        Train train = gameLogic.TrainMaster.GetRef(trainGuid);
        Assert.AreEqual(station1Guid, train.TravelPlan.SourceStation);
        Assert.AreEqual(station2Guid, train.TravelPlan.DestinationStation);
    }

    [Test]
    public void GameLogic_OnTrainArrival_IsExpectedBehaviour()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid station1Guid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid station2Guid = gameLogic.StationMaster.GetAll().ToList()[1];
        Guid trainGuid = gameLogic.TrainMaster.GetAll().ToList()[0];
        gameLogic.SetTrainTravelPlan(trainGuid, station1Guid, station2Guid);

        int numOfTestCargo = 10;
        gameLogic.TrainMaster.GetObject(trainGuid).Attribute.Capacity.UpperLimit = numOfTestCargo;
        Assert.IsTrue(gameLogic.CargoMaster.GetAll().Count == 0);

        gameLogic.AddRandomCargoToStation(station1Guid, numOfTestCargo);
        Assert.IsTrue(gameLogic.CargoMaster.GetAll().Count == numOfTestCargo);

        HashSet<Guid> guids = gameLogic.StationMaster.GetRef(station1Guid).CargoHelper.GetAll();
        Assert.IsTrue(gameLogic.TrainMaster.GetRef(trainGuid).CargoHelper.GetAll().Count == 0);

        foreach (var guid in guids)
        {
            gameLogic.RemoveCargoFromStation(station1Guid, guid);
            gameLogic.AddCargoToTrain(trainGuid, guid);
        }
        Assert.IsTrue(gameLogic.TrainMaster.GetRef(trainGuid).CargoHelper.GetAll().Count == numOfTestCargo);

        Assert.IsTrue(gameLogic.StationMaster.GetRef(station2Guid).TrainHelper.GetAll().Count == 0);
        gameLogic.OnTrainArrival(trainGuid);
        Assert.IsTrue(gameLogic.TrainMaster.GetRef(trainGuid).CargoHelper.GetAll().Count == 0);
        Assert.IsTrue(gameLogic.CargoMaster.GetAll().Count == 0);
        Assert.IsTrue(gameLogic.StationMaster.GetRef(station2Guid).TrainHelper.GetAll().Count == 1);
    }
    
    [Test]
    public void GameLogic_OnTrainDeparture_IsExpectedBehaviour()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid station1Guid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid station2Guid = gameLogic.StationMaster.GetAll().ToList()[1];
        Guid trainGuid = gameLogic.TrainMaster.GetAll().ToList()[0];
        gameLogic.SetTrainTravelPlan(trainGuid, station2Guid, station1Guid);
        gameLogic.OnTrainArrival(trainGuid);
        Assert.IsTrue(gameLogic.StationMaster.GetRef(station1Guid).TrainHelper.GetAll().Count == 1);

        gameLogic.OnTrainDeparture(trainGuid, station1Guid, station2Guid);
        Assert.IsTrue(gameLogic.TrainMaster.GetRef(trainGuid).TravelPlan.DestinationStation == station2Guid);
        Assert.IsTrue(gameLogic.StationMaster.GetRef(station1Guid).TrainHelper.GetAll().Count == 0);
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

    [Test]
    public void GameLogic_AddCargoToTrain_CargoAddedToTrain()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid stationGuid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid trainGuid = gameLogic.TrainMaster.GetAll().ToList()[0];
        gameLogic.AddRandomCargoToStation(stationGuid, 10);
        Guid cargoGuid = gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().ToList()[0];

        Assert.IsFalse(gameLogic.TrainMaster.GetRef(trainGuid).CargoHelper.GetAll().Contains(cargoGuid));
        gameLogic.AddCargoToTrain(trainGuid, cargoGuid);
        Assert.IsTrue(gameLogic.TrainMaster.GetRef(trainGuid).CargoHelper.GetAll().Contains(cargoGuid));
    }

    [Test]
    public void GameLogic_RemoveCargoFromTrain_CargoRemovedFromTrain()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid stationGuid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid trainGuid = gameLogic.TrainMaster.GetAll().ToList()[0];
        gameLogic.AddRandomCargoToStation(stationGuid, 10);
        Guid cargoGuid = gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().ToList()[0];
        gameLogic.RemoveCargoFromStation(stationGuid, cargoGuid);
        gameLogic.AddCargoToTrain(trainGuid, cargoGuid);

        Assert.IsTrue(gameLogic.TrainMaster.GetRef(trainGuid).CargoHelper.GetAll().Contains(cargoGuid));
        gameLogic.RemoveCargoFromTrain(trainGuid, cargoGuid);
        Assert.IsFalse(gameLogic.TrainMaster.GetRef(trainGuid).CargoHelper.GetAll().Contains(cargoGuid));
    }


    // Initializers
    private GameLogic GameLogicInit() => new();
    private GameLogic GameLogicWithCargoModelInit()
    {
        GameLogic gameLogic = GameLogicInit();
        gameLogic.GenerateCargoModels();
        return gameLogic;
    }
    private GameLogic GameLogicWithStationsInit()
    {
        GameLogic gameLogic = GameLogicWithCargoModelInit();
        Guid station1Guid = gameLogic.InitStation("Station1", new());
        Guid station2Guid = gameLogic.InitStation("Station2", new());
        gameLogic.AddTrack(station1Guid, station2Guid, StationOrientation.Head_Tail);
        return gameLogic;
    }
    private GameLogic GameLogicWithStationsAndTrainInit()
    {
        GameLogic gameLogic = GameLogicWithStationsInit();
        gameLogic.InitTrain("Train1", 5.0, new(), new(), TrainDirection.NORTH);
        return gameLogic;
    }
}
