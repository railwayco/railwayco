using NUnit.Framework;
using System;

public class PlatformMasterTests
{
    [Test]
    public void PlatformMaster_PlatformMaster_IsJsonSerialisedCorrectly()
    {
        PlatformMaster platformMaster = PlatformMasterWithPlatformInit();

        string jsonString = GameDataManager.Serialize(platformMaster);
        PlatformMaster platformMasterToVerify = GameDataManager.Deserialize<PlatformMaster>(jsonString);

        Assert.AreEqual(platformMaster, platformMasterToVerify);
    }

    [Test]
    public void PlatformMaster_AddPlatform_PlatformIsAdded()
    {
        PlatformMaster platformMaster = PlatformMasterInit();
        Platform platform = new(1, 1);

        platformMaster.AddPlatform(platform);

        Guid platformGuid = platformMaster.GetPlatformGuidByStationAndPlatformNum(1, 1);
        Assert.AreNotEqual(default, platformGuid);
    }

    [Test]
    public void PlatformMaster_GetPlatformsByStationNum_PlatformMatchesStationNum()
    {
        PlatformMaster platformMaster = PlatformMasterWithPlatformInit();
        Assert.IsTrue(platformMaster.GetPlatformsByStationNum(1).Count == 1);
        Assert.IsTrue(platformMaster.GetPlatformsByStationNum(2).Count == 1);
        Assert.IsTrue(platformMaster.GetPlatformsByStationNum(3) == default);
    }

    [Test]
    public void PlatformMaster_AddPlatformTrack_TrackExists()
    {
        PlatformMaster platformMaster = PlatformMasterInit();
        Platform sourcePlatform = new(1, 1);
        Platform destinationPlatform = new(2, 1);
        platformMaster.AddPlatform(sourcePlatform);
        platformMaster.AddPlatform(destinationPlatform);
        platformMaster.AddPlatformTrack(sourcePlatform.Guid,
                                        destinationPlatform.Guid,
                                        DepartDirection.West,
                                        DepartDirection.West);

        Track track = platformMaster.GetPlatformTrack(sourcePlatform.Guid, destinationPlatform.Guid);
        Assert.AreNotEqual(default, track);
    }

    [Test]
    public void PlatformMaster_GetPlatformTracks_AllTracksAccountedFor()
    {
        PlatformMaster platformMaster = PlatformMasterWithPlatformInit();
        Guid platform = platformMaster.GetPlatformGuidByStationAndPlatformNum(1, 1);
        Assert.IsTrue(platformMaster.GetPlatformTracks(platform).Count == 1);
    }

    [Test]
    public void PlatformMaster_GetPlatformTrack_TrackExists()
    {
        PlatformMaster platformMaster = PlatformMasterWithPlatformInit();
        Guid sourcePlatform = platformMaster.GetPlatformGuidByStationAndPlatformNum(1, 1);
        Guid destinationPlatform = platformMaster.GetPlatformGuidByStationAndPlatformNum(2, 1);
        Assert.IsTrue(platformMaster.GetPlatformTrack(sourcePlatform, destinationPlatform) != default);
    }

    [Test]
    public void PlatformMaster_GetPlatformGuidByStationAndPlatformNum_PlatformExists()
    {
        PlatformMaster platformMaster = PlatformMasterWithPlatformInit();
        Guid platform = platformMaster.GetPlatformGuidByStationAndPlatformNum(1, 1);
        Assert.IsTrue(platform != default);
    }

    private PlatformMaster PlatformMasterInit()
    {
        PlatformMaster platformMaster = new();
        return platformMaster;
    }

    private PlatformMaster PlatformMasterWithPlatformInit()
    {
        PlatformMaster platformMaster = PlatformMasterInit();
        Platform sourcePlatform = new(1, 1);
        Platform destinationPlatform = new(2, 1);
        platformMaster.AddPlatform(sourcePlatform);
        platformMaster.AddPlatform(destinationPlatform);
        platformMaster.AddPlatformTrack(sourcePlatform.Guid,
                                        destinationPlatform.Guid,
                                        DepartDirection.West,
                                        DepartDirection.West);
        return platformMaster;
    }
}
