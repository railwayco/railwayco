using NUnit.Framework;
using System;
using System.Collections.Generic;

public class StationMasterTests
{
    public StationMaster StationMaster { get; private set; }
    public PlatformMaster PlatformMaster { get; private set; }

    [SetUp]
    public void Init()
    {
        StationMaster = new();
        PlatformMaster = new();
    }

    public Guid[] AddTestStation(int numStations)
    {
        Guid[] stationGuids = new Guid[numStations];
        for (int i = 1; i <= numStations; i++)
        {
            Guid stationGuid = StationMaster.AddObject(i);
            stationGuids[i - 1] = stationGuid;
        }
        return stationGuids;
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
    public void StationMaster_AddObject_ObjectAddedToCollection()
    {
        int numStations = 5;
        Guid[] stationGuids = new Guid[numStations];
        for (int i = 1; i <= numStations; i++)
        {
            Guid stationGuid = StationMaster.AddObject(i);
            stationGuids[i - 1] = stationGuid;
        }

        foreach (Guid stationGuid in stationGuids)
        {
            Station station = StationMaster.GetObject(stationGuid);
            Assert.AreNotEqual(default, station);
        }
    }

    [Test]
    public void StationMaster_GetAllGuids_AllGuidsRetrieved()
    {
        Guid[] stationGuids = AddTestStation(5);
        HashSet<Guid> guids = StationMaster.GetAllGuids();
        foreach (Guid stationGuid in stationGuids)
        {
            Assert.IsTrue(guids.Remove(stationGuid));
        }
        Assert.IsEmpty(guids);
    }

    [Test]
    public void StationMaster_GetObject_ObjectAbleToBeRetrievedByGuid()
    {
        Guid[] stationGuids = AddTestStation(5);
        foreach (Guid stationGuid in stationGuids)
        {
            Station station = StationMaster.GetObject(stationGuid);
            Assert.AreNotEqual(default, station);
        }
    }

    [Test]
    public void StationMaster_GetObject_ObjectAbleToBeRetrievedByStationNum()
    {
        Guid[] stationGuids = AddTestStation(5);
        foreach (Guid stationGuid in stationGuids)
        {
            Station station = StationMaster.GetObject(stationGuid);
            int stationNum = station.Number;
            station = StationMaster.GetObject(stationNum);
            Assert.AreNotEqual(default, station);
        }
    }
    #endregion

    #region Cargo Management
    [Test]    
    public void StationMaster_GetRandomDestinations_RandomDestinationsExist()
    {
        Guid[] stationGuids = AddTestStation(5);
        Guid[] otherGuids = SimulateGuids(5);
        int numDestinations = 5;
        foreach (Guid stationGuid in stationGuids)
        {
            foreach (Guid otherGuid in otherGuids)
            {
                StationMaster.AddStationToStation(stationGuid, otherGuid);
            }

            IEnumerator<Guid> destinations = StationMaster.GetRandomDestinations(stationGuid, numDestinations);
            int count = 0;
            while (destinations.MoveNext())
                count++;
            Assert.IsTrue(count != 0);
        }
    }

    [Test]
    public void StationMaster_GetRandomDestinations_AreDifferent()
    {
        Guid[] stationGuids = AddTestStation(5);
        Guid[] otherGuids = SimulateGuids(5);
        int numDestinations = 1000;
        foreach (Guid stationGuid in stationGuids)
        {
            foreach (Guid otherGuid in otherGuids)
            {
                StationMaster.AddStationToStation(stationGuid, otherGuid);
            }

            IEnumerator<Guid> destinations = StationMaster.GetRandomDestinations(stationGuid, numDestinations);
            destinations.MoveNext();
            Guid destinationGuid = destinations.Current;
            int count = 0;
            while (destinations.MoveNext())
            {
                if (destinations.Current == destinationGuid)
                    count++;
            }
            Assert.IsTrue(count < numDestinations);
        }
    }

    [Test]
    public void StationMaster_AddCargoToStation_CargoAddedToStation()
    {
        Guid[] stationGuids = AddTestStation(5);
        Guid[] cargoGuids = SimulateGuids(5);
        foreach (Guid stationGuid in stationGuids)
        {
            foreach (Guid cargoGuid in cargoGuids)
            {
                StationMaster.AddCargoToStation(stationGuid, cargoGuid);
                Station station = StationMaster.GetObject(stationGuid);
                Assert.IsTrue(station.StationCargoHelper.Contains(cargoGuid));
            }
        }
    }

    [Test]
    public void StationMaster_RemoveCargoFromStation_CargoRemovedFromStation()
    {
        Guid[] stationGuids = AddTestStation(5);
        Guid[] cargoGuids = SimulateGuids(5);
        foreach (Guid stationGuid in stationGuids)
        {
            foreach (Guid cargoGuid in cargoGuids)
            {
                StationMaster.AddCargoToStation(stationGuid, cargoGuid);
                Station station = StationMaster.GetObject(stationGuid);
                Assert.IsTrue(station.StationCargoHelper.Contains(cargoGuid));

                StationMaster.RemoveCargoFromStation(stationGuid, cargoGuid);
                station = StationMaster.GetObject(stationGuid);
                Assert.IsFalse(station.StationCargoHelper.Contains(cargoGuid));
            }
        }
    }

    [Test]
    public void StationMaster_AddCargoToYard_CargoAddedToYard()
    {
        Guid[] stationGuids = AddTestStation(5);
        Guid[] cargoGuids = SimulateGuids(5); // Will break if changes made to YardCapacity
        foreach (Guid stationGuid in stationGuids)
        {
            foreach (Guid cargoGuid in cargoGuids)
            {
                Station station = StationMaster.GetObject(stationGuid);
                int yardCapacity = station.Attribute.YardCapacity.Amount;
                StationMaster.AddCargoToYard(stationGuid, cargoGuid);

                station = StationMaster.GetObject(stationGuid);
                Assert.IsTrue(station.YardCargoHelper.Contains(cargoGuid));
                Assert.AreEqual(yardCapacity + 1, station.Attribute.YardCapacity.Amount);
            }
        }
    }

    [Test]
    public void StationMaster_RemoveCargoFromYard_CargoRemovedFromYard()
    {
        Guid[] stationGuids = AddTestStation(5);
        Guid[] cargoGuids = SimulateGuids(5);
        foreach (Guid stationGuid in stationGuids)
        {
            foreach (Guid cargoGuid in cargoGuids)
            {
                Station station = StationMaster.GetObject(stationGuid);
                int yardCapacity = station.Attribute.YardCapacity.Amount;

                StationMaster.AddCargoToYard(stationGuid, cargoGuid);
                station = StationMaster.GetObject(stationGuid);
                Assert.IsTrue(station.YardCargoHelper.Contains(cargoGuid));
                Assert.AreEqual(yardCapacity + 1, station.Attribute.YardCapacity.Amount);

                StationMaster.RemoveCargoFromYard(stationGuid, cargoGuid);
                station = StationMaster.GetObject(stationGuid);
                Assert.IsFalse(station.YardCargoHelper.Contains(cargoGuid));
                Assert.AreEqual(yardCapacity, station.Attribute.YardCapacity.Amount);
            }
        }
    }

    [Test]
    public void StationMaster_GetStationCargoManifest_AllStationCargoPresent()
    {
        Guid[] stationGuids = AddTestStation(5);
        Guid[] cargoGuids = SimulateGuids(5);
        foreach (Guid stationGuid in stationGuids)
        {
            foreach (Guid cargoGuid in cargoGuids)
            {
                StationMaster.AddCargoToStation(stationGuid, cargoGuid);
            }

            HashSet<Guid> cargos = StationMaster.GetStationCargoManifest(stationGuid);
            foreach (Guid cargoGuid in cargoGuids)
            {
                Assert.IsTrue(cargos.Remove(cargoGuid));
            }
            Assert.IsEmpty(cargos);
        }
    }

    [Test]
    public void StationMaster_GetYardCargoManifest_AllYardCargoPresent()
    {
        Guid[] stationGuids = AddTestStation(5);
        Guid[] cargoGuids = SimulateGuids(5);
        foreach (Guid stationGuid in stationGuids)
        {
            foreach (Guid cargoGuid in cargoGuids)
            {
                StationMaster.AddCargoToYard(stationGuid, cargoGuid);
            }

            HashSet<Guid> cargos = StationMaster.GetYardCargoManifest(stationGuid);
            foreach (Guid cargoGuid in cargoGuids)
            {
                Assert.IsTrue(cargos.Remove(cargoGuid));
            }
            Assert.IsEmpty(cargos);
        }
    }
    #endregion

    #region Train Management
    [Test]
    public void StationMaster_AddTrainToStation_TrainAddedToStation()
    {
        Guid[] stationGuids = AddTestStation(5);
        Guid[] trainGuids = SimulateGuids(5);
        foreach (Guid stationGuid in stationGuids)
        {
            foreach (Guid trainGuid in trainGuids)
            {
                StationMaster.AddTrainToStation(stationGuid, trainGuid);
                Station station = StationMaster.GetObject(stationGuid);
                Assert.IsTrue(station.TrainHelper.Contains(trainGuid));
            }
        }
    }

    [Test]
    public void StationMaster_RemoveTrainFromStation_TrainRemovedFromStation()
    {
        Guid[] stationGuids = AddTestStation(5);
        Guid[] trainGuids = SimulateGuids(5);
        foreach (Guid stationGuid in stationGuids)
        {
            foreach (Guid trainGuid in trainGuids)
            {
                StationMaster.AddTrainToStation(stationGuid, trainGuid);
                Station station = StationMaster.GetObject(stationGuid);
                Assert.IsTrue(station.TrainHelper.Contains(trainGuid));

                StationMaster.RemoveTrainFromStation(stationGuid, trainGuid);
                station = StationMaster.GetObject(stationGuid);
                Assert.IsFalse(station.TrainHelper.Contains(trainGuid));
            }
        }
    }
    #endregion

    #region Station Management
    [Test]
    public void StationMaster_AddStationToStation_StationAddedToStation()
    {
        Guid[] stationGuids = AddTestStation(5);
        Guid[] otherGuids = SimulateGuids(5);
        foreach (Guid stationGuid in stationGuids)
        {
            foreach (Guid otherGuid in otherGuids)
            {
                StationMaster.AddStationToStation(stationGuid, otherGuid);
                Station station = StationMaster.GetObject(stationGuid);
                Assert.IsTrue(station.StationHelper.Contains(otherGuid));
            }
        }
    }

    [Test]
    public void StationMaster_RemoveStationFromStation_StationRemovedFromStation()
    {
        Guid[] stationGuids = AddTestStation(5);
        Guid[] otherGuids = SimulateGuids(5);
        foreach (Guid stationGuid in stationGuids)
        {
            foreach (Guid otherGuid in otherGuids)
            {
                StationMaster.AddStationToStation(stationGuid, otherGuid);
                Station station = StationMaster.GetObject(stationGuid);
                Assert.IsTrue(station.StationHelper.Contains(otherGuid));

                StationMaster.RemoveStationFromStation(stationGuid, otherGuid);
                station = StationMaster.GetObject(stationGuid);
                Assert.IsFalse(station.StationHelper.Contains(otherGuid));
            }
        }
    }
    #endregion
}
