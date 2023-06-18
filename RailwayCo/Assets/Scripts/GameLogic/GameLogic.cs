using System;
using System.Collections.Generic;

public class GameLogic
{
    private User User { get; set; }
    private CargoMaster CargoMaster { get; set; }
    private TrainMaster TrainMaster { get; set; }
    private StationMaster StationMaster { get; set; }
    private CargoCatalog CargoCatalog { get; set; }
    private TrainCatalog TrainCatalog { get; set; }

    public GameLogic(
        User user,
        CargoMaster cargoMaster,
        TrainMaster trainMaster,
        StationMaster stationMaster,
        CargoCatalog cargoCatalog,
        TrainCatalog trainCatalog)
    {
        User = user;
        CargoMaster = cargoMaster;
        TrainMaster = trainMaster;
        StationMaster = stationMaster;
        CargoCatalog = cargoCatalog;
        TrainCatalog = trainCatalog;
    }

    public void MoveCargoFromStationtoTrain(Guid cargo, Guid station, Guid train)
    {
        StationMaster.RemoveCargo(station, cargo);
        TrainMaster.AddCargo(train, cargo);
    }

    public void MoveCargoFromTrainToStation(Guid cargo, Guid train, Guid station)
    {
        TrainMaster.RemoveCargo(train, cargo);
        StationMaster.AddCargo(station, cargo);
    }

    public void OnTrainArrival(Guid train)
    {
        Guid station = TrainMaster.GetDestination(train);
        StationMaster.AddTrain(station, train);

        HashSet<Guid> cargoCollection = TrainMaster.GetAllCargo(train);
        cargoCollection = CargoMaster.FilterCargoHasArrived(cargoCollection, station);

        CurrencyManager total = CargoMaster.GetCurrencyManagerForCargoRange(cargoCollection);
        User.CurrencyManager.AddCurrencyManager(total);

        TrainMaster.RemoveCargoRange(train, cargoCollection);
        CargoMaster.RemoveCargoRange(cargoCollection);
    }

    public void OnTrainDeparture(Guid train, Guid sourceStation, Guid destinationStation)
    {
        // TODO: Check if train has sufficient fuel and durability

        TrainMaster.SetTravelPlan(train, sourceStation, destinationStation);
        StationMaster.RemoveTrain(sourceStation, train);
    }
}
