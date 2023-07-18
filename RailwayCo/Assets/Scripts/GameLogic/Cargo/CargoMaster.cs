using System;
using System.Collections.Generic;
using System.Linq;

public class CargoMaster
{
    private WorkerDictHelper<Cargo> Collection { get; set; }
    private WorkerDictHelper<CargoModel> CargoCatalog { get; set; }

    public CargoMaster()
    {
        Collection = new();
        CargoCatalog = new();

        InitCargoCatalog();
    }

    #region Collection Management
    public Guid AddObject(CargoModel cargoModel, Guid sourceStation, Guid destinationStation)
    {
        Cargo cargo = new(cargoModel, sourceStation, destinationStation);
        Collection.Add(cargo);
        return cargo.Guid;
    }
    public void RemoveObject(Guid cargo) => Collection.Remove(cargo);
    public Cargo GetObject(Guid cargo) => Collection.GetRef(cargo);
    #endregion

    #region CargoCatalog Management
    private void InitCargoCatalog()
    {
        Random rand = new();
        CargoType[] cargoTypes = (CargoType[])Enum.GetValues(typeof(CargoType));
        CurrencyType[] currencyTypes = (CurrencyType[])Enum.GetValues(typeof(CurrencyType));

        foreach (var cargoType in cargoTypes)
        {
            RangedCurrencyManager rangedCurrencyManager = new();
            CurrencyType randomType = currencyTypes[rand.Next(currencyTypes.Length)];

            switch (randomType)
            {
                case CurrencyType.Coin:
                    rangedCurrencyManager.SetCurrencyRanged(randomType, 10, 100);
                    break;
                case CurrencyType.Note:
                    rangedCurrencyManager.SetCurrencyRanged(randomType, 1, 5);
                    break;
                case CurrencyType.NormalCrate:
                    rangedCurrencyManager.SetCurrencyRanged(randomType, 1, 1);
                    break;
                case CurrencyType.SpecialCrate:
                    rangedCurrencyManager.SetCurrencyRanged(randomType, 1, 1);
                    break;
                default:
                    rangedCurrencyManager.SetCurrencyRanged(randomType, 1, 1);
                    break;
            }

            CargoModel cargoModel = new(cargoType, 15, 20, rangedCurrencyManager);
            CargoCatalog.Add(cargoModel);
        }
    }
    public IEnumerable<CargoModel> GetRandomCargoModels(int numCargoModels)
    {
        List<Guid> keys = CargoCatalog.GetAll().ToList();
        int totalCargoModels = keys.Count;
        if (totalCargoModels == 0)
            yield break;

        Random rand = new();
        for (int i = 0; i < numCargoModels; i++)
        {
            int randomIndex = rand.Next(totalCargoModels);

            Guid randomGuid = keys[randomIndex];
            CargoModel cargoModel = CargoCatalog.GetRef(randomGuid);
            cargoModel.Randomise();

            yield return cargoModel;
        }
    }
    #endregion

    #region CargoAssociation Management
    public void SetCargoAssociation(Guid cargo, CargoAssociation cargoAssoc)
    {
        Cargo cargoObject = Collection.GetObject(cargo);
        cargoObject.CargoAssoc = cargoAssoc;
    }
    #endregion

    #region TravelPlan Management
    public bool HasCargoArrived(Guid cargo, Guid station)
    {
        Cargo cargoObject = Collection.GetObject(cargo);
        return cargoObject.TravelPlan.HasArrived(station);
    }
    public bool IsCargoAtSource(Guid cargo, Guid station)
    {
        Cargo cargoObject = Collection.GetObject(cargo);
        return cargoObject.TravelPlan.IsAtSource(station);
    }
    #endregion

    #region CurrencyManager Management
    public CurrencyManager GetCurrencyManager(Guid cargo)
    {
        Cargo cargoObject = Collection.GetObject(cargo);
        return cargoObject.CurrencyManager;
    }
    #endregion
}
