using NUnit.Framework;
using System;

public class TravelPlanTests
{
    [Test]
    public void TravelPlan_TravelPlan_IsJsonSerialisedCorrectly()
    {
        Guid source = Guid.NewGuid();
        Guid destination = Guid.NewGuid();
        TravelPlan travelPlan = TravelPlanInit(source, destination);

        string jsonString = GameDataManager.Serialize(travelPlan);
        TravelPlan travelPlanToVerify = GameDataManager.Deserialize<TravelPlan>(jsonString);

        Assert.AreEqual(travelPlan, travelPlanToVerify);
    }

    [Test]
    public void TravelPlan_HasArrived_IsCorrect()
    {
        Guid source = Guid.NewGuid();
        Guid destination = Guid.NewGuid();
        TravelPlan travelPlan = TravelPlanInit(source, destination);

        Assert.IsTrue(travelPlan.HasArrived(destination));
        Assert.IsFalse(travelPlan.HasArrived(source));
    }

    [Test]
    public void TravelPlan_IsAtSource_IsCorrect()
    {
        Guid source = Guid.NewGuid();
        Guid destination = Guid.NewGuid();
        TravelPlan travelPlan = TravelPlanInit(source, destination);

        Assert.IsTrue(travelPlan.IsAtSource(source));
        Assert.IsFalse(travelPlan.IsAtSource(destination));
    }

    private TravelPlan TravelPlanInit(Guid source, Guid destination)
    {
        TravelPlan travelPlan = new(source, destination);
        return travelPlan;
    }
}
