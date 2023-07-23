using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class CargoMasterTests
{
    public CargoMaster CargoMaster { get; private set; }

    [SetUp]
    public void Init() => CargoMaster = new();

    public Guid[] AddTestCargo(int numCargos)
    {
        Guid[] cargoGuids = new Guid[numCargos];
        IEnumerable<CargoModel> cargoModels = CargoMaster.GetRandomCargoModels(numCargos);
        Guid source = Guid.NewGuid();
        Guid destination = Guid.NewGuid();
        int index = 0;
        foreach (var cargoModel in cargoModels)
        {
            Guid cargoGuid = CargoMaster.AddObject(cargoModel, source, destination);
            cargoGuids[index] = cargoGuid;
            index++;
        }
        return cargoGuids;
    }

    #region Collection Management
    [Test]
    public void CargoMaster_AddObject_ObjectAddedToCollection()
    {
        int numCargos = 5;
        Guid[] cargoGuids = new Guid[numCargos];

        IEnumerable<CargoModel> cargoModels = CargoMaster.GetRandomCargoModels(numCargos);
        Guid source = Guid.NewGuid();
        Guid destination = Guid.NewGuid();
        int index = 0;
        foreach (var cargoModel in cargoModels)
        {
            Guid cargoGuid = CargoMaster.AddObject(cargoModel, source, destination);
            cargoGuids[index] = cargoGuid;
            index++;
        }

        foreach (Guid cargoGuid in cargoGuids)
        {
            Cargo cargo = CargoMaster.GetObject(cargoGuid);
            Assert.AreNotEqual(default, cargo);
        }
    }

    [Test]
    public void CargoMaster_RemoveObject_ObjectRemovedFromCollection()
    {
        Guid[] cargoGuids = AddTestCargo(5);
        foreach (Guid cargoGuid in cargoGuids)
        {
            Cargo cargo = CargoMaster.GetObject(cargoGuid);
            Assert.AreNotEqual(default, cargo);

            CargoMaster.RemoveObject(cargoGuid);
            cargo = CargoMaster.GetObject(cargoGuid);
            Assert.AreEqual(default, cargo);
        }
    }

    [Test]
    public void CargoMaster_GetObject_ObjectAbleToBeRetrieved()
    {
        Guid[] cargoGuids = AddTestCargo(5);
        foreach (Guid cargoGuid in cargoGuids)
        {
            Cargo cargo = CargoMaster.GetObject(cargoGuid);
            Assert.AreNotEqual(default, cargo);
        }
    }
    #endregion

    #region CargoCatalog Management
    [Test]
    public void CargoMaster_GetRandomCargoModels_RandomCargoModelsExist()
    {
        int numCargos = 5;
        IEnumerable<CargoModel> cargoModels = CargoMaster.GetRandomCargoModels(numCargos);
        Assert.IsTrue(cargoModels.Any());
    }

    [Test]
    public void CargoMaster_GetRandomCargoModels_AreDifferent()
    {
        int numCargos = 1000;
        IEnumerable<CargoModel> cargoModels = CargoMaster.GetRandomCargoModels(numCargos);
        Guid cargoModelGuid = cargoModels.First().Guid;
        Assert.That(cargoModels.Count(c => c.Guid == cargoModelGuid), Is.LessThan(numCargos));
    }
    #endregion

    #region CargoAssociation Management
    [Test]
    [TestCase(CargoAssociation.Nil)]
    [TestCase(CargoAssociation.Station)]
    [TestCase(CargoAssociation.Train)]
    [TestCase(CargoAssociation.Yard)]
    public void CargoMaster_SetCargoAssociation_CorrectCargoAssociationSet(CargoAssociation cargoAssoc)
    {
        Guid[] cargoGuids = AddTestCargo(5);
        foreach (Guid cargoGuid in cargoGuids)
        {
            CargoMaster.SetCargoAssociation(cargoGuid, cargoAssoc);
            Cargo cargo = CargoMaster.GetObject(cargoGuid);
            Assert.AreEqual(cargoAssoc, cargo.CargoAssoc);
        }
    }
    #endregion

    #region TravelPlan Management
    [Test]
    public void CargoMaster_HasCargoArrived_ChecksAgainstDestination()
    {
        Guid[] cargoGuids = AddTestCargo(5);
        foreach (Guid cargoGuid in cargoGuids)
        {
            Cargo cargo = CargoMaster.GetObject(cargoGuid);
            Guid source = cargo.TravelPlan.SourceStation;
            Guid destination = cargo.TravelPlan.DestinationStation;
            Assert.AreEqual(false, CargoMaster.HasCargoArrived(cargoGuid, source));
            Assert.AreEqual(true, CargoMaster.HasCargoArrived(cargoGuid, destination));
        }
    }

    [Test]
    public void CargoMaster_IsCargoAtSource_ChecksAgainstSource()
    {
        Guid[] cargoGuids = AddTestCargo(5);
        foreach (Guid cargoGuid in cargoGuids)
        {
            Cargo cargo = CargoMaster.GetObject(cargoGuid);
            Guid source = cargo.TravelPlan.SourceStation;
            Guid destination = cargo.TravelPlan.DestinationStation;
            Assert.AreEqual(true, CargoMaster.IsCargoAtSource(cargoGuid, source));
            Assert.AreEqual(false, CargoMaster.IsCargoAtSource(cargoGuid, destination));
        }
    }
    #endregion

    #region CurrencyManager Management
    public void CargoMaster_GetCurrencyManager_CurrencyManagerExists()
    {
        Guid[] cargoGuids = AddTestCargo(5);
        foreach (Guid cargoGuid in cargoGuids)
        {
            CurrencyManager currencyManager = CargoMaster.GetCurrencyManager(cargoGuid);
            Assert.AreNotEqual(default, currencyManager);
        }
    }
    #endregion
}
