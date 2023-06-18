using System;
using System.Collections.Generic;

public class GameLogic
{
    private User User { get; set; }
    private CargoMaster CargoMaster { get; set; }
    private TrainMaster TrainMaster { get; set; }
    private StationMaster StationMaster { get; set; }

    public void MoveCargoFromStationtoTrain(Guid cargoGuid, Guid stationGuid, Guid trainGuid)
    {
        StationMaster.RemoveCargo(stationGuid, cargoGuid);
        TrainMaster.AddCargo(trainGuid, cargoGuid);
    }

    public void MoveCargoFromTrainToStation(Guid cargoGuid, Guid trainGuid, Guid stationGuid)
    {
        TrainMaster.RemoveCargo(trainGuid, cargoGuid);
        StationMaster.AddCargo(stationGuid, cargoGuid);
    }

    public void OnTrainArrival(Guid trainGuid)
    {
        Guid stationGuid = TrainMaster.GetDestination(trainGuid);
        HashSet<Guid> cargoCollection = TrainMaster.GetAllCargo(trainGuid);
        cargoCollection = CargoMaster.FilterCargoHasArrived(cargoCollection, stationGuid);

        CurrencyManager total = CargoMaster.GetCurrencyManagerForCargoRange(cargoCollection);
        User.CurrencyManager.AddCurrencyManager(total);

        TrainMaster.RemoveCargoRange(trainGuid, cargoCollection);
        CargoMaster.RemoveCargoRange(cargoCollection);
    }

    public void OnTrainDeparture()
    {

    }
}
