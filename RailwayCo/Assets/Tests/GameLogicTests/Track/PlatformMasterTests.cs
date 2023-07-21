using NUnit.Framework;
using System;

public class PlatformMasterTests
{
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
                                        destinationPlatform.Guid);

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

    [Test]
    public void PlatformMaster_GetPlatformStationNum_PlatformExists()
    {
        PlatformMaster platformMaster = PlatformMasterWithPlatformInit();
        Guid platform_1_1 = platformMaster.GetPlatformGuidByStationAndPlatformNum(1, 1);
        Guid platform_2_1 = platformMaster.GetPlatformGuidByStationAndPlatformNum(2, 1);
        Assert.IsTrue(platformMaster.GetPlatformStationNum(platform_1_1) == 1);
        Assert.IsTrue(platformMaster.GetPlatformStationNum(platform_2_1) == 2);
    }

    [Test]
    public void PlatformMaster_GetPlatform_PlatformExists()
    {
        PlatformMaster platformMaster = PlatformMasterWithPlatformInit();
        Guid platform_1_1 = platformMaster.GetPlatformGuidByStationAndPlatformNum(1, 1);
        Assert.IsTrue(platformMaster.GetPlatform(platform_1_1) != default);
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
                                        destinationPlatform.Guid);
        return platformMaster;
    }
}
