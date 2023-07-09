using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

public class StationReacherTests
{
    [TestCaseSource(nameof(StationMasterTestCases))]
    public void StationReacher_Bfs_IsCorrect(
        WorkerDictHelper<Station> stationMaster,
        DictHelper<HashsetHelper> reacherDict)
    {
        StationReacher stationReacher = StationReacherInit();
        stationReacher.Bfs(stationMaster);

        HashSet<Guid> guids = stationReacher.ReacherDict.GetAll();
        foreach (var guid in guids)
        {
            HashsetHelper expected = reacherDict.GetObject(guid);
            HashsetHelper actual = stationReacher.ReacherDict.GetObject(guid);
            Assert.AreEqual(expected, actual);
        }
    }

    private static IEnumerable<TestCaseData> StationMasterTestCases
    {
        get
        {
            WorkerDictHelper<Station> stationMaster_Loop = StationMaster_3Stations_Loop();
            WorkerDictHelper<Station> stationMaster_AllConnected = StationMaster_3Stations_AllConnected();
            WorkerDictHelper<Station> stationMaster_2Connected = StationMaster_3Stations_2Connected();
            WorkerDictHelper<Station> stationMaster_AllDisconnected = StationMaster_3Stations_AllDisconnected();

            yield return new TestCaseData(stationMaster_Loop, AllConnectedResult(stationMaster_Loop));
            yield return new TestCaseData(stationMaster_AllConnected, AllConnectedResult(stationMaster_AllConnected));
            yield return new TestCaseData(stationMaster_2Connected, TwoConnectedResult(stationMaster_2Connected));
            yield return new TestCaseData(stationMaster_AllDisconnected, AllDisconnectedResult(stationMaster_AllDisconnected));
        }
    }

    private static WorkerDictHelper<Station> StationMaster_3Stations_Loop()
    {
        GameLogic gameLogic = new();
        Guid station1 = gameLogic.InitStation("Station1", new());
        Guid station2 = gameLogic.InitStation("Station2", new());
        Guid station3 = gameLogic.InitStation("Station3", new());

        gameLogic.AddStationLinks(station1, station2);
        gameLogic.AddStationLinks(station2, station3);
        gameLogic.AddStationLinks(station3, station1);

        return gameLogic.StationMaster;
    }
    private static WorkerDictHelper<Station> StationMaster_3Stations_AllConnected()
    {
        GameLogic gameLogic = new();
        Guid station1 = gameLogic.InitStation("Station1", new());
        Guid station2 = gameLogic.InitStation("Station2", new());
        Guid station3 = gameLogic.InitStation("Station3", new());

        gameLogic.AddStationLinks(station1, station2);
        gameLogic.AddStationLinks(station2, station3);

        return gameLogic.StationMaster;
    }
    private static DictHelper<HashsetHelper> AllConnectedResult(WorkerDictHelper<Station> stationMaster)
    {
        Guid station1 = Guid.Empty;
        Guid station2 = Guid.Empty;
        Guid station3 = Guid.Empty;

        List<Guid> guids = stationMaster.GetAll().ToList();
        foreach (var guid in guids)
        {
            if (stationMaster.GetRef(guid).Name == "Station1")
                station1 = guid;
            else if (stationMaster.GetRef(guid).Name == "Station2")
                station2 = guid;
            else if (stationMaster.GetRef(guid).Name == "Station3")
                station3 = guid;
        }

        DictHelper<HashsetHelper> reacherDict = new();
        reacherDict.Add(station1, new());
        reacherDict.Add(station2, new());
        reacherDict.Add(station3, new());

        reacherDict.GetObject(station1).Add(station2);
        reacherDict.GetObject(station1).Add(station3);
        reacherDict.GetObject(station2).Add(station1);
        reacherDict.GetObject(station2).Add(station3);
        reacherDict.GetObject(station3).Add(station1);
        reacherDict.GetObject(station3).Add(station2);

        return reacherDict;
    }

    private static WorkerDictHelper<Station> StationMaster_3Stations_2Connected()
    {
        GameLogic gameLogic = new();
        Guid station1 = gameLogic.InitStation("Station1", new());
        Guid station2 = gameLogic.InitStation("Station2", new());
        gameLogic.InitStation("Station3", new());

        gameLogic.AddStationLinks(station1, station2);

        return gameLogic.StationMaster;
    }
    private static DictHelper<HashsetHelper> TwoConnectedResult(WorkerDictHelper<Station> stationMaster)
    {
        Guid station1 = Guid.Empty;
        Guid station2 = Guid.Empty;
        Guid station3 = Guid.Empty;

        List<Guid> guids = stationMaster.GetAll().ToList();
        foreach (var guid in guids)
        {
            if (stationMaster.GetRef(guid).Name == "Station1")
                station1 = guid;
            else if (stationMaster.GetRef(guid).Name == "Station2")
                station2 = guid;
            else if (stationMaster.GetRef(guid).Name == "Station3")
                station3 = guid;
        }

        DictHelper<HashsetHelper> reacherDict = new();
        reacherDict.Add(station1, new());
        reacherDict.Add(station2, new());
        reacherDict.Add(station3, new());

        reacherDict.GetObject(station1).Add(station2);
        reacherDict.GetObject(station2).Add(station1);

        return reacherDict;
    }

    private static WorkerDictHelper<Station> StationMaster_3Stations_AllDisconnected()
    {
        GameLogic gameLogic = new();
        gameLogic.InitStation("Station1", new());
        gameLogic.InitStation("Station2", new());
        gameLogic.InitStation("Station3", new());

        return gameLogic.StationMaster;
    }
    private static DictHelper<HashsetHelper> AllDisconnectedResult(WorkerDictHelper<Station> stationMaster)
    {
        Guid station1 = Guid.Empty;
        Guid station2 = Guid.Empty;
        Guid station3 = Guid.Empty;

        List<Guid> guids = stationMaster.GetAll().ToList();
        foreach (var guid in guids)
        {
            if (stationMaster.GetRef(guid).Name == "Station1")
                station1 = guid;
            else if (stationMaster.GetRef(guid).Name == "Station2")
                station2 = guid;
            else if (stationMaster.GetRef(guid).Name == "Station3")
                station3 = guid;
        }

        DictHelper<HashsetHelper> reacherDict = new();
        reacherDict.Add(station1, new());
        reacherDict.Add(station2, new());
        reacherDict.Add(station3, new());

        return reacherDict;
    }

    private StationReacher StationReacherInit()
    {
        StationReacher stationReacher = new(new());
        return stationReacher;
    }
}
