using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Intermediary between all the GameObjects and Backend GameManeger/GameLogic
public class LogicManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    private void Awake()
    {
        if (!_gameManager) Debug.LogError("Game Manager is not attached to the logic manager!");
    }

    //////////////////////////////////////////////////////
    /// SETUP RELATED
    //////////////////////////////////////////////////////

    // Either retrive old station GUID or create a new GUID
    public Guid SetupGetStationGUID(out bool isNewStation, GameObject stationGO)
    {
        Vector3 position = stationGO.transform.position;
        Station station = _gameManager.GameLogic.GetStationRefByPosition(position);

        if (station is null)
        {
            isNewStation = true;
            return  _gameManager.GameLogic.InitStation(stationGO.name, position);
        }
        else
        {
            isNewStation = false;
            return station.Guid;
        }
    }

    public void StationGenerateTracks(string stationName)
    {
        _gameManager.GameLogic.GenerateTracks(stationName);
    }

    // Either Retrieve old train GUID or create a new GUID
    // TODO: Once the ability to add new trains by the user is supported, the initial load should only load existing trains from the DB
    public Guid SetupGetTrainGUID(TrainMovement trainMovScript, GameObject trainGO)
    {
        TrainDirection movementDirn = trainMovScript.MovementDirn;
        Vector3 trainPosition = trainMovScript.transform.position;
        Quaternion trainRotation = trainMovScript.transform.rotation;
        float maxSpeed = trainMovScript.MaxSpeed;

        Vector3 position = trainGO.transform.position;
        Train train = GetTrainClassObject(position);
        if (train == null)
        {
            return _gameManager.GameLogic.InitTrain(trainGO.name, maxSpeed, trainPosition, trainRotation, movementDirn);
        }
        else
        {
            return train.Guid;
        }
    }

    //////////////////////////////////////////////////////
    /// TRAIN RELATED
    //////////////////////////////////////////////////////

    public Train GetTrainClassObject(Vector3 position)
    {
        return _gameManager.GameLogic.GetTrainRefByPosition(position);
    }

    public void UpdateTrainBackend(TrainMovement trainMovScript,Guid trainGuid)
    {
        float trainCurrentSpeed = trainMovScript.CurrentSpeed;
        TrainDirection movementDirn = trainMovScript.MovementDirn;
        Vector3 trainPosition = trainMovScript.transform.position;
        Quaternion trainRotation = trainMovScript.transform.rotation;

        Train trainClassObject = _gameManager.GameLogic.TrainMaster.GetObject(trainGuid);
        trainClassObject.Attribute.SetUnityStats(trainCurrentSpeed, trainPosition, trainRotation, movementDirn);
    }

    public void SetTrainTravelPlan(Guid trainGuid, Guid srcStnGuid, Guid dstStnGuid)
    {
        _gameManager.GameLogic.SetTrainTravelPlan(trainGuid, srcStnGuid, dstStnGuid);
    }

    //////////////////////////////////////////////////////
    /// STATION RELATED
    //////////////////////////////////////////////////////
    public void SetStationAsDestination(Guid trainGUID, Guid currentStationGUID, Guid destinationStationGUID)
    {
        _gameManager.GameLogic.OnTrainDeparture(trainGUID, currentStationGUID, destinationStationGUID);
    }

    public Guid FindImmediateStationNeighbour(Guid currentStationGuid, bool findLeftNeighbour)
    {
        Station stationObject = GetIndividualStation(currentStationGuid);
        HashSet<Guid> neighbourGuids = stationObject.StationHelper.GetAll();
        foreach (Guid neighbour in neighbourGuids)
        {
            StationOrientation neighbourOrientation = stationObject.StationHelper.GetObject(neighbour);

            if (findLeftNeighbour)
            {
                if (neighbourOrientation == StationOrientation.Tail_Tail ||
                    neighbourOrientation == StationOrientation.Tail_Head)
                {
                    return neighbour;
                }
            }
            else
            {
                if (neighbourOrientation == StationOrientation.Head_Head ||
                    neighbourOrientation == StationOrientation.Head_Tail)
                {
                    return neighbour;
                }
            }
        }
        return Guid.Empty;
    }

    public Station GetIndividualStation(Guid stationGUID)
    {
        return _gameManager.GameLogic.StationMaster.GetRef(stationGUID);
    }

    //////////////////////////////////////////////////////
    /// CARGO LIST RETRIEVAL
    //////////////////////////////////////////////////////

    public List<Cargo> GetTrainCargoList(Guid trainGUID)
    {
        if (trainGUID == Guid.Empty)
        {
            Debug.LogError("Invalid trainGUID to get associated cargo");
            return null;
        }

        Train trainRef = _gameManager.GameLogic.TrainMaster.GetRef(trainGUID);
        HashSet<Guid> cargoHashset = trainRef.CargoHelper.GetAll();
        return GetCargoListFromGUIDs(cargoHashset);
    }

    // Gets either the Yard Cargo or the station cargo
    public List<Cargo> GetSelectedStationCargoList(Guid stationGUID, bool getStationCargo)
    {
        // Gets all the station AND yard cargo, since they are under the same cargoHelper in the station
        HashSet<Guid> cargoHashset = _gameManager.GameLogic.StationMaster.GetRef(stationGUID).CargoHelper.GetAll();

        if (cargoHashset.Count == 0) { 
            // Generate a new set of Cargo if that station is empty
            _gameManager.GameLogic.AddRandomCargoToStation(stationGUID, 10);
            cargoHashset = _gameManager.GameLogic.StationMaster.GetRef(stationGUID).CargoHelper.GetAll();
        }

        List<Cargo> allStationCargo = GetCargoListFromGUIDs(cargoHashset);
        return getStationSubCargo(allStationCargo, getStationCargo);
    }

    /// By default, the call to get the station cargo returns both (new) station cargo and also yard cargo
    /// This functions serves to return the sub-category of the cargo
    /// Returns Either the station cargo or the yard cargo</returns>
    private List<Cargo> getStationSubCargo(List<Cargo> allStationCargo, bool getStation)
    {
        List<Cargo> output = new List<Cargo>();
        foreach (Cargo cargo in allStationCargo)
        {
            CargoAssociation cargoAssoc = cargo.CargoAssoc;
            if (getStation && cargoAssoc == CargoAssociation.STATION) // Get Station-Only cargo
            {
                output.Add(cargo);
            }
            else if (!getStation && cargoAssoc == CargoAssociation.YARD)// Get Yard-Only cargo
            {
                output.Add(cargo);
            }
            else continue;
        }
        return output;
    }

    private List<Cargo> GetCargoListFromGUIDs(HashSet<Guid> cargoHashset)
    {
        List<Cargo> cargoList = new List<Cargo>();
        foreach (Guid guid in cargoHashset)
        {
            cargoList.Add(_gameManager.GameLogic.CargoMaster.GetRef(guid));
        }
        return cargoList;
    }

    //////////////////////////////////////////////////////
    /// CARGO PROCESSING AND SHIFTING
    //////////////////////////////////////////////////////

    public void ProcessCargoOnTrainStop(Guid trainGUID)
    {
        _gameManager.GameLogic.OnTrainArrival(trainGUID);
        Transform statsPanel = GameObject.Find("MainUI").transform.Find("BottomPanel").Find("UI_StatsPanel");
        int exp = _gameManager.GameLogic.User.ExperiencePoint;
        
        CurrencyManager currMgr = _gameManager.GameLogic.User.CurrencyManager;
        Currency curr;
        currMgr.CurrencyDict.TryGetValue(CurrencyType.Coin, out curr);
        double coinVal = curr.CurrencyValue;

        currMgr.CurrencyDict.TryGetValue(CurrencyType.Note, out curr);
        double noteVal = curr.CurrencyValue;

        currMgr.CurrencyDict.TryGetValue(CurrencyType.NormalCrate, out curr);
        double normalCrateVal = curr.CurrencyValue;

        currMgr.CurrencyDict.TryGetValue(CurrencyType.SpecialCrate, out curr);
        double specialCrateVal = curr.CurrencyValue;

        statsPanel.Find("EXPText").GetComponent<Text>().text = exp.ToString();
        statsPanel.Find("CoinText").GetComponent<Text>().text = coinVal.ToString();
        statsPanel.Find("NoteText").GetComponent<Text>().text = noteVal.ToString();
        statsPanel.Find("NormalCrateText").GetComponent<Text>().text = normalCrateVal.ToString();
        statsPanel.Find("SpecialCrateText").GetComponent<Text>().text = specialCrateVal.ToString();
    }

    public void ShiftCargoOnButtonClick(GameObject cargoDetailButtonGO, Cargo cargo, Guid currentTrainGUID, Guid currentStationGUID)
    {
        CargoAssociation cargoAssoc = cargo.CargoAssoc;
        if (cargoAssoc == CargoAssociation.STATION || cargoAssoc == CargoAssociation.YARD)
        {
            ShiftCargoFromStationOrYardToTrain(cargo.Guid, currentTrainGUID, currentStationGUID);
            // TODO: Check if can add to station before removing from train
            Destroy(cargoDetailButtonGO);
        }
        else if (cargoAssoc == CargoAssociation.TRAIN)
        {
            ShiftCargoFromTrainToStationOrYard(cargo.Guid, currentTrainGUID, currentStationGUID);
            // TODO: Check if can add to station before removing from train
            Destroy(cargoDetailButtonGO);
        }
        else
        {
            Debug.LogError($"There is currently no logic being implemented for CargoAssociation {cargoAssoc}");
        }
    }


    private void ShiftCargoFromTrainToStationOrYard(Guid cargoGUID, Guid currentTrainGUID, Guid currentStationGUID)
    {
        _gameManager.GameLogic.RemoveCargoFromTrain(currentTrainGUID, cargoGUID);
        _gameManager.GameLogic.AddCargoToStation(currentStationGUID, cargoGUID);
    }

    private void ShiftCargoFromStationOrYardToTrain(Guid cargoGUID, Guid currentTrainGUID, Guid currentStationGUID)
    {
        _gameManager.GameLogic.RemoveCargoFromStation(currentStationGUID, cargoGUID);
        _gameManager.GameLogic.AddCargoToTrain(currentTrainGUID, cargoGUID);
    }
}
