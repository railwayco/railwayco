using System.Collections.Generic;
using NUnit.Framework;

public class CargoManagerTests
{
    [TestCaseSource("CargoStubs")]
    public void CargoManager_AddCargo_CargoIsAdded(Cargo cargo)
    {
        CargoManager cargoManager = CargoManagerInit();
        cargoManager.AddCargo(cargo);
        Assert.IsTrue(cargoManager.CargoList.Contains(cargo));
    }

    [TestCase("Station1")]
    [TestCase("Station2")]
    [TestCase("Station3")]
    public void CargoManager_GetArrivedCargo_CorrectCargoFiltered(string targetStation)
    {
        CargoManager cargoManager = CargoManagerInit();
        List<Cargo> cargos = Cargos;
        foreach (var cargo in cargos)
        {
            cargoManager.AddCargo(cargo);
        }

        List<Cargo> actual = cargoManager.GetArrivedCargo(targetStation);
        List<Cargo> expected = cargos.FindAll((cargo) => cargo.DestinationStation.StationName == targetStation);
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestCase("Station1")]
    [TestCase("Station2")]
    [TestCase("Station3")]
    public void CargoManager_RemoveSelectedCargo_CorrectCargoRemoved(string targetStation)
    {
        CargoManager cargoManager = CargoManagerInit();
        List<Cargo> cargos = Cargos;
        foreach (var cargo in cargos)
        {
            cargoManager.AddCargo(cargo);
        }

        List<Cargo> cargosToRemove = cargos.FindAll((cargo) => cargo.DestinationStation.StationName == targetStation);
        cargoManager.RemoveSelectedCargo(cargosToRemove);

        List<Cargo> expected = cargos.FindAll((cargo) => cargo.DestinationStation.StationName != targetStation);
        CollectionAssert.AreEqual(expected, cargoManager.CargoList);
    }

    public CargoManager CargoManagerInit() => new();

    public static List<TestCaseData> CargoStubs
    {
        get
        {
            List<Cargo> cargos = Cargos;
            List<TestCaseData> testCases = new();
            foreach(var cargo in cargos)
            {
                testCases.Add(new TestCaseData(cargo));
            }
            return testCases;
        }
    }

    public static List<Cargo> Cargos
    {
        get
        {
            return new List<Cargo>()
            {
                new Cargo(
                    CargoType.Wood,
                    new(0, 0, 100, 0),
                    new(),
                    new("Station1", StationStatus.Open),
                    new("Station2", StationStatus.Open)),
                new Cargo(
                    CargoType.Wood,
                    new(0, 0, 200, 0),
                    new(),
                    new("Station2", StationStatus.Open),
                    new("Station3", StationStatus.Open)),
                new Cargo(
                    CargoType.Wood,
                    new(0, 0, 200, 0),
                    new(),
                    new("Station1", StationStatus.Open),
                    new("Station3", StationStatus.Open))
            };
        }     
    }
}
