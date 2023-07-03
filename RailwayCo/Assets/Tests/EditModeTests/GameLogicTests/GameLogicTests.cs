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

        Station stationObject = gameLogic.StationMaster.GetObject(stationGuid);
        Guid cargoGuid = stationObject.CargoHelper.GetAll().ToList()[0];
        gameLogic.RemoveCargoFromStation(stationGuid, cargoGuid);
        gameLogic.AddCargoToTrain(trainGuid, cargoGuid);

        Cargo cargoObject = gameLogic.CargoMaster.GetObject(cargoGuid);

        Assert.IsFalse(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsFalse(cargoObject.CargoAssoc == CargoAssociation.STATION);
        gameLogic.AddCargoToStation(stationGuid, cargoGuid);
        Assert.IsTrue(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsTrue(cargoObject.CargoAssoc == CargoAssociation.STATION);
    }

    [Test]
    public void GameLogic_AddCargoToStation_CargoAddedToYard()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid station1Guid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid station2Guid = gameLogic.StationMaster.GetAll().ToList()[1];
        Guid trainGuid = gameLogic.TrainMaster.GetAll().ToList()[0];
        gameLogic.AddRandomCargoToStation(station1Guid, 10);

        Station stationObject = gameLogic.StationMaster.GetObject(station1Guid);
        Guid cargoGuid = stationObject.CargoHelper.GetAll().ToList()[0];
        gameLogic.RemoveCargoFromStation(station1Guid, cargoGuid);
        gameLogic.AddCargoToTrain(trainGuid, cargoGuid);

        int expectedYardCapacity = stationObject.Attribute.YardCapacity.Amount + 1;
        Cargo cargoObject = gameLogic.CargoMaster.GetObject(cargoGuid);
        cargoObject.TravelPlan.SetSourceStation(station2Guid);

        Assert.IsFalse(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsFalse(cargoObject.CargoAssoc == CargoAssociation.YARD);
        gameLogic.AddCargoToStation(station1Guid, cargoGuid);
        Assert.IsTrue(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsTrue(cargoObject.CargoAssoc == CargoAssociation.YARD);
        Assert.AreEqual(expectedYardCapacity, stationObject.Attribute.YardCapacity.Amount);
    }

    [Test]
    public void GameLogic_AddCargoToStation_CargoFailedToAddToYard()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid station1Guid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid station2Guid = gameLogic.StationMaster.GetAll().ToList()[1];
        Guid trainGuid = gameLogic.TrainMaster.GetAll().ToList()[0];
        gameLogic.AddRandomCargoToStation(station1Guid, 2);

        Station stationObject = gameLogic.StationMaster.GetObject(station1Guid);
        stationObject.Attribute.YardCapacity.UpperLimit = 1;

        Guid cargo1Guid = stationObject.CargoHelper.GetAll().ToList()[0];
        Guid cargo2Guid = stationObject.CargoHelper.GetAll().ToList()[1];
        gameLogic.RemoveCargoFromStation(station1Guid, cargo1Guid);
        gameLogic.AddCargoToTrain(trainGuid, cargo1Guid);
        gameLogic.RemoveCargoFromStation(station1Guid, cargo2Guid);
        gameLogic.AddCargoToTrain(trainGuid, cargo2Guid);

        int expectedYardCapacity = stationObject.Attribute.YardCapacity.Amount + 1;
        Cargo cargoObject1 = gameLogic.CargoMaster.GetObject(cargo1Guid);
        Cargo cargoObject2 = gameLogic.CargoMaster.GetObject(cargo2Guid);
        cargoObject1.TravelPlan.SetSourceStation(station2Guid);
        cargoObject2.TravelPlan.SetSourceStation(station2Guid);
        gameLogic.RemoveCargoFromTrain(trainGuid, cargo1Guid);
        gameLogic.RemoveCargoFromTrain(trainGuid, cargo2Guid);

        Assert.IsFalse(stationObject.CargoHelper.GetAll().Contains(cargo1Guid));
        Assert.IsFalse(stationObject.CargoHelper.GetAll().Contains(cargo2Guid));
        Assert.IsFalse(cargoObject1.CargoAssoc == CargoAssociation.YARD);
        Assert.IsFalse(cargoObject2.CargoAssoc == CargoAssociation.YARD);
        gameLogic.AddCargoToStation(station1Guid, cargo1Guid);
        gameLogic.AddCargoToStation(station1Guid, cargo2Guid);
        Assert.IsTrue(stationObject.CargoHelper.GetAll().Contains(cargo1Guid));
        Assert.IsFalse(stationObject.CargoHelper.GetAll().Contains(cargo2Guid));
        Assert.IsTrue(cargoObject1.CargoAssoc == CargoAssociation.YARD);
        Assert.IsFalse(cargoObject2.CargoAssoc == CargoAssociation.YARD);
        Assert.AreEqual(expectedYardCapacity, stationObject.Attribute.YardCapacity.Amount);
    }

    [Test]
    public void GameLogic_RemoveCargoFromStation_CargoRemovedFromStation()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid stationGuid = gameLogic.StationMaster.GetAll().ToList()[0];
        Station stationObject = gameLogic.StationMaster.GetObject(stationGuid);
        gameLogic.AddRandomCargoToStation(stationGuid, 10);
        Guid cargoGuid = gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().ToList()[0];
        Cargo cargoObject = gameLogic.CargoMaster.GetObject(cargoGuid);

        Assert.IsTrue(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsFalse(cargoObject.CargoAssoc == CargoAssociation.NIL);
        gameLogic.RemoveCargoFromStation(stationGuid, cargoGuid);
        Assert.IsFalse(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
    }

    [Test]
    public void GameLogic_RemoveCargoFromStation_CargoRemovedFromYard()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid station1Guid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid station2Guid = gameLogic.StationMaster.GetAll().ToList()[1];
        gameLogic.AddRandomCargoToStation(station1Guid, 10);
        Guid cargoGuid = gameLogic.StationMaster.GetRef(station1Guid).CargoHelper.GetAll().ToList()[0];

        Station stationObject = gameLogic.StationMaster.GetObject(station1Guid);
        Cargo cargoObject = gameLogic.CargoMaster.GetObject(cargoGuid);
        cargoObject.TravelPlan.SetSourceStation(station2Guid);
        gameLogic.AddCargoToStation(station1Guid, cargoGuid);
        int expectedYardCapacity = stationObject.Attribute.YardCapacity.Amount - 1;

        Assert.IsTrue(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsFalse(cargoObject.CargoAssoc == CargoAssociation.NIL);
        gameLogic.RemoveCargoFromStation(station1Guid, cargoGuid);
        Assert.IsFalse(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.AreEqual(expectedYardCapacity, stationObject.Attribute.YardCapacity.Amount);
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
        gameLogic.RemoveCargoFromStation(stationGuid, cargoGuid);

        Train trainObject = gameLogic.TrainMaster.GetObject(trainGuid);

        Assert.IsFalse(trainObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsTrue(trainObject.Attribute.Capacity.Amount == 0);
        gameLogic.AddCargoToTrain(trainGuid, cargoGuid);
        Assert.IsTrue(trainObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsTrue(trainObject.Attribute.Capacity.Amount == 1);
    }

    [Test]
    public void GameLogic_AddCargoToTrain_CargoFailedToAddToTrain()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid stationGuid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid trainGuid = gameLogic.TrainMaster.GetAll().ToList()[0];
        gameLogic.AddRandomCargoToStation(stationGuid, 10);
        Guid cargo1Guid = gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().ToList()[0];
        gameLogic.RemoveCargoFromStation(stationGuid, cargo1Guid);
        Guid cargo2Guid = gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().ToList()[1];
        gameLogic.RemoveCargoFromStation(stationGuid, cargo2Guid);

        Train trainObject = gameLogic.TrainMaster.GetObject(trainGuid);
        trainObject.Attribute.Capacity.UpperLimit = 1;

        Assert.IsFalse(trainObject.CargoHelper.GetAll().Contains(cargo1Guid));
        Assert.IsFalse(trainObject.CargoHelper.GetAll().Contains(cargo2Guid));
        Assert.IsTrue(trainObject.Attribute.Capacity.Amount == 0);
        gameLogic.AddCargoToTrain(trainGuid, cargo1Guid);
        gameLogic.AddCargoToTrain(trainGuid, cargo2Guid);
        Assert.IsTrue(trainObject.CargoHelper.GetAll().Contains(cargo1Guid));
        Assert.IsFalse(trainObject.CargoHelper.GetAll().Contains(cargo2Guid));
        Assert.IsTrue(trainObject.Attribute.Capacity.Amount == 1);
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

        Train trainObject = gameLogic.TrainMaster.GetObject(trainGuid);

        Assert.IsTrue(trainObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsTrue(trainObject.Attribute.Capacity.Amount == 1);
        gameLogic.RemoveCargoFromTrain(trainGuid, cargoGuid);
        Assert.IsFalse(trainObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsTrue(trainObject.Attribute.Capacity.Amount == 0);
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
