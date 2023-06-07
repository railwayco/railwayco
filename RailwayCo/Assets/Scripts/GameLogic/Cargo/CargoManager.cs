using System.Collections.Generic;

public class CargoManager
{
    private List<Cargo> cargoList;

    public List<Cargo> CargoList { get => cargoList; private set => cargoList = value; }

    public CargoManager() => CargoList = new();

    public void AddCargo(Cargo cargo) => CargoList.Add(cargo);

    public List<Cargo> GetArrivedCargo(string stationName)
    {
        return CargoList.FindAll(c => c.DestinationStation.StationName == stationName);
    }

    public void RemoveSelectedCargo(List<Cargo> cargoList)
    {
        foreach (Cargo cargo in cargoList)
        {
            CargoList.Remove(cargo);
        }
    }
}
