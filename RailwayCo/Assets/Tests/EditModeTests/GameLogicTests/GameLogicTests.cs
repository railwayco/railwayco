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
        Guid stationGuid = gameLogic.InitStation(1, new());
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
        Assert.IsFalse(cargoObject.CargoAssoc == CargoAssociation.Station);
        gameLogic.AddCargoToStation(stationGuid, cargoGuid);
        Assert.IsTrue(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsTrue(cargoObject.CargoAssoc == CargoAssociation.Station);
    }

    [Test]
    public void GameLogic_AddCargoToStation_CargoAddedToYard()
    {
        GameLogic gameLogic = GameLogicWithStationsInit();
        Guid station1Guid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid station2Guid = gameLogic.StationMaster.GetAll().ToList()[1];
        gameLogic.AddRandomCargoToStation(station2Guid, 10);

        Guid cargoGuid = gameLogic.StationMaster.GetRef(station2Guid).CargoHelper.GetAll().ToList()[0];
        Cargo cargoObject = gameLogic.CargoMaster.GetObject(cargoGuid);

        Station stationObject = gameLogic.StationMaster.GetObject(station1Guid);
        int expectedYardCapacity = stationObject.Attribute.YardCapacity.Amount + 1;

        Assert.IsFalse(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsFalse(cargoObject.CargoAssoc == CargoAssociation.Yard);
        gameLogic.AddCargoToStation(station1Guid, cargoGuid);
        Assert.IsTrue(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsTrue(cargoObject.CargoAssoc == CargoAssociation.Yard);
        Assert.AreEqual(expectedYardCapacity, stationObject.Attribute.YardCapacity.Amount);
    }

    [Test]
    public void GameLogic_AddCargoToStation_CargoFailedToAddToYard()
    {
        GameLogic gameLogic = GameLogicWithStationsInit();
        Guid station1Guid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid station2Guid = gameLogic.StationMaster.GetAll().ToList()[1];

        Station stationObject = gameLogic.StationMaster.GetObject(station1Guid);
        int yardCapacity = stationObject.Attribute.YardCapacity.UpperLimit;

        gameLogic.AddRandomCargoToStation(station2Guid, yardCapacity + 1);
        List<Guid> cargoList = gameLogic.StationMaster.GetRef(station2Guid).CargoHelper.GetAll().ToList();

        for (int i = 0; i < yardCapacity; i++)
        {
            Guid cargoGuid = cargoList[i];
            Cargo cargoObject = gameLogic.CargoMaster.GetObject(cargoGuid);

            Assert.IsFalse(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
            Assert.IsFalse(cargoObject.CargoAssoc == CargoAssociation.Yard);
            gameLogic.AddCargoToStation(station1Guid, cargoGuid);
            Assert.IsTrue(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
            Assert.IsTrue(cargoObject.CargoAssoc == CargoAssociation.Yard);
        }

        Guid failedCargoGuid = cargoList[yardCapacity];
        Cargo failedCargo = gameLogic.CargoMaster.GetObject(failedCargoGuid);

        Assert.IsFalse(stationObject.CargoHelper.GetAll().Contains(failedCargoGuid));
        Assert.IsFalse(failedCargo.CargoAssoc == CargoAssociation.Yard);
        gameLogic.AddCargoToStation(station1Guid, failedCargoGuid);
        Assert.IsFalse(stationObject.CargoHelper.GetAll().Contains(failedCargoGuid));
        Assert.IsFalse(failedCargo.CargoAssoc == CargoAssociation.Yard);
        Assert.AreEqual(yardCapacity, stationObject.Attribute.YardCapacity.Amount);
    }

    [Test]
    public void GameLogic_RemoveCargoFromStation_CargoRemovedFromStation()
    {
        GameLogic gameLogic = GameLogicWithStationsInit();
        Guid stationGuid = gameLogic.StationMaster.GetAll().ToList()[0];
        Station stationObject = gameLogic.StationMaster.GetObject(stationGuid);
        gameLogic.AddRandomCargoToStation(stationGuid, 10);
        Guid cargoGuid = gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().ToList()[0];
        Cargo cargoObject = gameLogic.CargoMaster.GetObject(cargoGuid);

        Assert.IsTrue(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsFalse(cargoObject.CargoAssoc == CargoAssociation.Nil);
        gameLogic.RemoveCargoFromStation(stationGuid, cargoGuid);
        Assert.IsFalse(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
    }

    [Test]
    public void GameLogic_RemoveCargoFromStation_CargoRemovedFromYard()
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid station1Guid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid station2Guid = gameLogic.StationMaster.GetAll().ToList()[1];
        gameLogic.AddRandomCargoToStation(station2Guid, 10);
        Guid cargoGuid = gameLogic.StationMaster.GetRef(station2Guid).CargoHelper.GetAll().ToList()[0];

        Station stationObject = gameLogic.StationMaster.GetObject(station1Guid);
        Cargo cargoObject = gameLogic.CargoMaster.GetObject(cargoGuid);
        gameLogic.AddCargoToStation(station1Guid, cargoGuid);
        int expectedYardCapacity = stationObject.Attribute.YardCapacity.Amount - 1;

        Assert.IsTrue(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.IsFalse(cargoObject.CargoAssoc == CargoAssociation.Nil);
        gameLogic.RemoveCargoFromStation(station1Guid, cargoGuid);
        Assert.IsFalse(stationObject.CargoHelper.GetAll().Contains(cargoGuid));
        Assert.AreEqual(expectedYardCapacity, stationObject.Attribute.YardCapacity.Amount);
    }

    // Train related tests
    [Test]
    public void GameLogic_InitTrain_IsTrainAdded()
    {
        GameLogic gameLogic = GameLogicInit();
        Guid trainGuid = gameLogic.InitTrain("Train1", 5.0, new(), new(), DepartDirection.North);
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

        int numOfTestCargo = gameLogic.TrainMaster.GetObject(trainGuid).Attribute.Capacity.UpperLimit;
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
        gameLogic.SetTrainTravelPlan(trainGuid, station1Guid, station2Guid);

        gameLogic.OnTrainDeparture(trainGuid);
        Assert.IsTrue(gameLogic.TrainMaster.GetRef(trainGuid).TravelPlan.DestinationStation == station2Guid);
        Assert.IsTrue(gameLogic.StationMaster.GetRef(station1Guid).TrainHelper.GetAll().Count == 0);
    }

    [TestCase(0F, 0F)]
    [TestCase(10F, 0F)]
    [TestCase(0F, 10F)]
    public void GameLogic_OnTrainDeparture_TrainUnableToDepart(double durability, double fuel)
    {
        GameLogic gameLogic = GameLogicWithStationsAndTrainInit();
        Guid station1Guid = gameLogic.StationMaster.GetAll().ToList()[0];
        Guid station2Guid = gameLogic.StationMaster.GetAll().ToList()[1];
        Guid trainGuid = gameLogic.TrainMaster.GetAll().ToList()[0];
        gameLogic.SetTrainTravelPlan(trainGuid, station2Guid, station1Guid);
        gameLogic.OnTrainArrival(trainGuid);
        Assert.IsTrue(gameLogic.StationMaster.GetRef(station1Guid).TrainHelper.GetAll().Count == 1);
        gameLogic.SetTrainTravelPlan(trainGuid, station1Guid, station2Guid);

        Train trainObject = gameLogic.TrainMaster.GetObject(trainGuid);
        trainObject.Attribute.Durability.Amount = durability;
        trainObject.Attribute.Fuel.Amount = fuel;

        gameLogic.OnTrainDeparture(trainGuid);
        Assert.IsFalse(gameLogic.StationMaster.GetRef(station1Guid).TrainHelper.GetAll().Count == 0);
    }

    [TestCase(1F, 2F, 3F)]
    [TestCase(-1F, 2F, -3.5F)]
    public void GameLogic_GetTrainRefByPosition_TrainCanBeFound(float x, float y, float z)
    {
        GameLogic gameLogic = GameLogicInit();
        Vector3 vector = new(x, y, z);
        Guid trainGuid = gameLogic.InitTrain("Train1", 5.0, vector, new(), DepartDirection.North);

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
        
        Train trainObject = gameLogic.TrainMaster.GetObject(trainGuid);
        int trainCapacity = trainObject.Attribute.Capacity.UpperLimit;

        gameLogic.AddRandomCargoToStation(stationGuid, trainCapacity + 1);
        List<Guid> cargoList = gameLogic.StationMaster.GetRef(stationGuid).CargoHelper.GetAll().ToList();

        Assert.IsTrue(trainObject.Attribute.Capacity.Amount == 0);
        for (int i = 0; i < trainCapacity; i++)
        {
            // Relies on trainObject.Attribute.Capacity.UpperLimit set in TrainInit
            Guid cargoGuid = cargoList[i];
            gameLogic.RemoveCargoFromStation(stationGuid, cargoGuid);

            Assert.IsFalse(trainObject.CargoHelper.GetAll().Contains(cargoGuid));
            gameLogic.AddCargoToTrain(trainGuid, cargoGuid);
            Assert.IsTrue(trainObject.CargoHelper.GetAll().Contains(cargoGuid));
        }

        Guid failCargoGuid = cargoList[trainCapacity];
        Assert.IsFalse(trainObject.CargoHelper.GetAll().Contains(failCargoGuid));
        gameLogic.AddCargoToTrain(trainGuid, failCargoGuid);
        Assert.IsFalse(trainObject.CargoHelper.GetAll().Contains(failCargoGuid));

        Assert.IsTrue(trainObject.Attribute.Capacity.Amount == trainCapacity);        
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
        gameLogic.InitStation(1, new());
        gameLogic.InitStation(2, new());
        return gameLogic;
    }
    private GameLogic GameLogicWithStationsAndTrainInit()
    {
        GameLogic gameLogic = GameLogicWithStationsInit();
        gameLogic.InitTrain("Train1", 5.0, new(), new(), DepartDirection.North);
        return gameLogic;
    }
}
