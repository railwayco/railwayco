using System;
using System.Collections.Generic;
using System.Linq;

public class CargoMaster : IPlayfab
{
    private WorkerDictHelper<Cargo> Collection { get; set; }
    private WorkerDictHelper<CargoModel> Catalog { get; set; }

    public CargoMaster()
    {
        Collection = new();
        Catalog = new();

        InitCatalog();
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
    private void InitCatalog()
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
                    rangedCurrencyManager.SetRangedCurrency(randomType, 10, 100);
                    break;
                case CurrencyType.Note:
                    rangedCurrencyManager.SetRangedCurrency(randomType, 1, 5);
                    break;
                case CurrencyType.NormalCrate:
                    rangedCurrencyManager.SetRangedCurrency(randomType, 1, 1);
                    break;
                case CurrencyType.SpecialCrate:
                    rangedCurrencyManager.SetRangedCurrency(randomType, 1, 1);
                    break;
                default:
                    rangedCurrencyManager.SetRangedCurrency(randomType, 1, 1);
                    break;
            }

            CargoModel cargoModel = new(cargoType, 15, 20, rangedCurrencyManager);
            Catalog.Add(cargoModel);
        }
    }
    public IEnumerable<CargoModel> GetRandomCargoModels(int numCargoModels)
    {
        List<Guid> keys = Catalog.GetAll().ToList();
        int totalCargoModels = keys.Count;
        if (totalCargoModels == 0)
            yield break;

        Random rand = new();
        for (int i = 0; i < numCargoModels; i++)
        {
            int randomIndex = rand.Next(totalCargoModels);

            Guid randomGuid = keys[randomIndex];
            CargoModel cargoModel = Catalog.GetRef(randomGuid);
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

    #region PlayFab Management
    public string SendDataToPlayfab() => GameDataManager.Serialize(Collection);
    public void SetDataFromPlayfab(string data)
    {
        Collection = GameDataManager.Deserialize<WorkerDictHelper<Cargo>>(data);
    }
    #endregion
}
