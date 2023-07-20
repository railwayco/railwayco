using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TrainMasterTests
{
    public TrainMaster TrainMaster { get; private set; }

    [SetUp]
    public void Init() => TrainMaster = new();

    public Guid[] AddTestTrain(int numTrains)
    {
        Guid[] trainGuids = new Guid[numTrains];
        for (int i = 0; i < numTrains; i++)
        {
            TrainType trainType = TrainType.Steam;
            Vector3 position = new(i, i + 1, i + 2);
            Guid trainGuid = TrainMaster.AddObject(trainType, 5, position, new(), DepartDirection.North);
            trainGuids[i] = trainGuid;
        }
        return trainGuids;
    }

    public Guid[] SimulateGuids(int numGuid)
    {
        Guid[] guids = new Guid[numGuid];
        for (int i = 0; i < numGuid; i++)
        {
            guids[i] = Guid.NewGuid();
        }
        return guids;
    }

    #region Collection Management
    [Test]
    public void TrainMaster_AddObject_ObjectAddedToCollection()
    {
        int numTrains = 5;
        for (int i = 0; i < numTrains; i++)
        {
            TrainType trainType = TrainType.Steam;
            Vector3 position = new(i, i + 1, i + 2);
            Guid trainGuid = TrainMaster.AddObject(trainType, 5, position, new(), DepartDirection.North);
            Train train = TrainMaster.GetObject(trainGuid);
            Assert.AreNotEqual(default, train);
        }
    }

    [Test]
    public void TrainMaster_RemoveObject_ObjectRemovedFromCollection()
    {
        int numTrains = 5;
        for (int i = 0; i < numTrains; i++)
        {
            Vector3 position = new(i, i + 1, i + 2);
            Guid trainGuid = TrainMaster.AddObject("", 5, position, new(), DepartDirection.North);
            Train train = TrainMaster.GetObject(trainGuid);
            Assert.AreNotEqual(default, train);

            TrainMaster.RemoveObject(trainGuid);
            Assert.Throws<NullReferenceException>(() => TrainMaster.GetObject(trainGuid));
        }
    }

    [Test]
    public void TrainMaster_GetObject_ObjectAbleToBeRetrievedByGuid()
    {
        Guid[] trainGuids = AddTestTrain(5);
        foreach (Guid trainGuid in trainGuids)
        {
            Train train = TrainMaster.GetObject(trainGuid);
            Assert.AreNotEqual(default, train);
        }
    }

    [Test]
    public void TrainMaster_GetObject_ObjectAbleToBeRetrievedByPosition()
    {
        Guid[] trainGuids = AddTestTrain(5);
        foreach (Guid trainGuid in trainGuids)
        {
            Train train = TrainMaster.GetObject(trainGuid);
            Vector3 position = train.Attribute.Position;
            train = TrainMaster.GetObject(position);
            Assert.AreNotEqual(default, train);
        }
    }
    #endregion

    #region Status Management
    [Test]
    public void TrainMaster_ActivateTrain_TrainStatusActive()
    {
        Guid[] trainGuids = AddTestTrain(5);
        foreach (Guid trainGuid in trainGuids)
        {
            TrainMaster.ActivateTrain(trainGuid);
            Train train = TrainMaster.GetObject(trainGuid);
            Assert.AreEqual(TrainStatus.Active, train.Status);
        }
    }

    [Test]
    public void TrainMaster_DeactivateTrain_TrainStatusInactive()
    {
        Guid[] trainGuids = AddTestTrain(5);
        foreach (Guid trainGuid in trainGuids)
        {
            TrainMaster.DeactivateTrain(trainGuid);
            Train train = TrainMaster.GetObject(trainGuid);
            Assert.AreEqual(TrainStatus.Inactive, train.Status);
        }
    }
    #endregion

    #region TrainAttribute Management
    [Test]
    public void TrainMaster_SetUnityStats_StatsAllSet()
    {
        Guid[] trainGuids = AddTestTrain(5);
        foreach (Guid trainGuid in trainGuids)
        {
            Train train = TrainMaster.GetObject(trainGuid);
            Vector3 oldPosition = train.Attribute.Position;
            Quaternion oldRotation = train.Attribute.Rotation;

            double newSpeed = train.Attribute.Speed.Amount + 10;
            Vector3 newPosition = new(oldPosition.z, oldPosition.x, oldPosition.y);
            Quaternion newRotation = new(oldRotation.w, oldRotation.z, oldRotation.y, oldRotation.x);
            DepartDirection newDirection = DepartDirection.West;

            TrainMaster.SetUnityStats(trainGuid, (float)newSpeed, newPosition, newRotation, newDirection);
            train = TrainMaster.GetObject(trainGuid);
            double verifySpeed = train.Attribute.Speed.Amount;
            Vector3 verifyPosition = train.Attribute.Position;
            Quaternion verifyRotation = train.Attribute.Rotation;
            DepartDirection verifyDirection = train.Attribute.Direction;

            Assert.AreEqual(newSpeed, verifySpeed);
            Assert.AreEqual(newPosition, verifyPosition);
            Assert.AreEqual(newRotation, verifyRotation);
            Assert.AreEqual(newDirection, verifyDirection);
        }
    }

    [Test]
    public void TrainMaster_Refuel_FuelIncreased()
    {
        Guid[] trainGuids = AddTestTrain(5);
        foreach (Guid trainGuid in trainGuids)
        {
            TrainMaster.BurnFuel(trainGuid);
            Train train = TrainMaster.GetObject(trainGuid);
            double oldFuel = train.Attribute.Fuel.Amount;

            TrainMaster.Refuel(trainGuid);
            train = TrainMaster.GetObject(trainGuid);
            Assert.IsTrue(train.Attribute.Fuel.Amount > oldFuel);
        }
    }

    [Test]
    public void TrainMaster_BurnFuel_FuelDecreased()
    {
        Guid[] trainGuids = AddTestTrain(5);
        foreach (Guid trainGuid in trainGuids)
        {
            Train train = TrainMaster.GetObject(trainGuid);
            double oldFuel = train.Attribute.Fuel.Amount;

            TrainMaster.BurnFuel(trainGuid);
            train = TrainMaster.GetObject(trainGuid);
            Assert.IsTrue(train.Attribute.Fuel.Amount < oldFuel);
        }
    }

    [Test]
    public void TrainMaster_Repair_DurabilityIncreased()
    {
        Guid[] trainGuids = AddTestTrain(5);
        foreach (Guid trainGuid in trainGuids)
        {
            TrainMaster.Wear(trainGuid);
            Train train = TrainMaster.GetObject(trainGuid);
            double oldDurability = train.Attribute.Durability.Amount;

            TrainMaster.Repair(trainGuid);
            train = TrainMaster.GetObject(trainGuid);
            Assert.IsTrue(train.Attribute.Durability.Amount > oldDurability);
        }
    }

    [Test]
    public void TrainMaster_Wear_DurabilityDecreased()
    {
        Guid[] trainGuids = AddTestTrain(5);
        foreach (Guid trainGuid in trainGuids)
        {
            Train train = TrainMaster.GetObject(trainGuid);
            double oldDurability = train.Attribute.Durability.Amount;

            TrainMaster.Wear(trainGuid);
            train = TrainMaster.GetObject(trainGuid);
            Assert.IsTrue(train.Attribute.Durability.Amount < oldDurability);
        }
    }
    #endregion

    #region TravelPlan Management
    [Test]
    public void TrainMaster_FileTravelPlan_TravelPlanSet()
    {
        Guid[] trainGuids = AddTestTrain(5);
        foreach (Guid trainGuid in trainGuids)
        {
            Guid source = Guid.NewGuid();
            Guid destination = Guid.NewGuid();

            TrainMaster.FileTravelPlan(trainGuid, source, destination);
            Train train = TrainMaster.GetObject(trainGuid);
            Assert.AreNotEqual(default, train.TravelPlan);
        }
    }
    
    [Test]
    public void TrainMaster_CompleteTravelPlan_TravelPlanRemoved()
    {
        Guid[] trainGuids = AddTestTrain(5);
        foreach (Guid trainGuid in trainGuids)
        {
            TrainMaster.CompleteTravelPlan(trainGuid);
            Train train = TrainMaster.GetObject(trainGuid);
            Assert.AreEqual(default, train.TravelPlan);
        }
    }

    [Test]
    public void TrainMaster_GetDepartureStation_CorrectStationReturned()
    {
        Guid[] trainGuids = AddTestTrain(5);
        foreach (Guid trainGuid in trainGuids)
        {
            Guid source = Guid.NewGuid();
            Guid destination = Guid.NewGuid();

            TrainMaster.FileTravelPlan(trainGuid, source, destination);
            Train train = TrainMaster.GetObject(trainGuid);
            Assert.AreNotEqual(default, train.TravelPlan);

            Guid sourceStation = TrainMaster.GetDepartureStation(trainGuid);
            Assert.AreEqual(source, sourceStation);
        }
    }

    [Test]
    public void TrainMaster_GetArrivalStation_CorrectStationReturned()
    {
        Guid[] trainGuids = AddTestTrain(5);
        foreach (Guid trainGuid in trainGuids)
        {
            Guid source = Guid.NewGuid();
            Guid destination = Guid.NewGuid();

            TrainMaster.FileTravelPlan(trainGuid, source, destination);
            Train train = TrainMaster.GetObject(trainGuid);
            Assert.AreNotEqual(default, train.TravelPlan);

            Guid destStation = TrainMaster.GetArrivalStation(trainGuid);
            Assert.AreEqual(destination, destStation);
        }
    }
    #endregion

    #region Cargo Management
    [Test]
    public void TrainMaster_AddCargoToTrain_CargoAddedToTrain()
    {
        Guid[] trainGuids = AddTestTrain(5);
        Guid[] cargoGuids = SimulateGuids(4); // Will break if changes made to Train Capacity
        foreach (Guid trainGuid in trainGuids)
        {
            foreach (Guid cargoGuid in cargoGuids)
            {
                TrainMaster.AddCargoToTrain(trainGuid, cargoGuid);
                Train train = TrainMaster.GetObject(trainGuid);
                Assert.IsTrue(train.CargoHelper.GetAll().Contains(cargoGuid));
            }
        }
    }

    [Test]
    public void TrainMaster_RemoveCargoFromTrain_CargoRemovedFromTrain()
    {
        Guid[] trainGuids = AddTestTrain(5);
        Guid[] cargoGuids = SimulateGuids(5);
        foreach (Guid trainGuid in trainGuids)
        {
            foreach (Guid cargoGuid in cargoGuids)
            {
                TrainMaster.AddCargoToTrain(trainGuid, cargoGuid);
                Train train = TrainMaster.GetObject(trainGuid);
                Assert.IsTrue(train.CargoHelper.GetAll().Contains(cargoGuid));

                TrainMaster.RemoveCargoFromTrain(trainGuid, cargoGuid);
                train = TrainMaster.GetObject(trainGuid);
                Assert.IsFalse(train.CargoHelper.GetAll().Contains(cargoGuid));
            }
        }
    }

    [Test]
    public void TrainMaster_GetCargoManifest_AllCargoPresent()
    {
        Guid[] trainGuids = AddTestTrain(5);
        Guid[] cargoGuids = SimulateGuids(4); // Will break if changes made to Train Capacity
        foreach (Guid trainGuid in trainGuids)
        {
            foreach (Guid cargoGuid in cargoGuids)
            {
                TrainMaster.AddCargoToTrain(trainGuid, cargoGuid);
                Train train = TrainMaster.GetObject(trainGuid);
                Assert.IsTrue(train.CargoHelper.GetAll().Contains(cargoGuid));
            }

            HashSet<Guid> cargos = TrainMaster.GetCargoManifest(trainGuid);
            foreach (Guid cargoGuid in cargoGuids)
            {
                Assert.IsTrue(cargos.Remove(cargoGuid));
            }
            Assert.IsEmpty(cargos);
        }
    }
    #endregion
}
