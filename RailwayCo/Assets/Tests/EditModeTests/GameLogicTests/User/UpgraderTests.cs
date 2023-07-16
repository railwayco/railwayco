using System;
using NUnit.Framework;

public class UpgraderTests
{
    [Test]
    public void Upgrader_Upgrader_IsJsonSerialisedCorrectly()
    {
        Upgrader upgrader = UpgraderInit(100);

        string jsonString = GameDataManager.Serialize(upgrader);
        Upgrader upgraderToVerify = GameDataManager.Deserialize<Upgrader>(jsonString);

        Assert.AreEqual(upgraderToVerify, upgrader);
    }
    
    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void Upgrader_AddSkillPoint_SkillPointIncreased(int basePoint, int skillPoint)
    {
        Upgrader upgrader = UpgraderInit(basePoint);
        upgrader.AddSkillPoint(skillPoint);
        Assert.AreEqual(Arithmetic.IntAddition(basePoint, skillPoint), upgrader.SkillPoint);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void Upgrader_AddSkillPoint_SkillPointInvalid(int basePoint, int skillPoint)
    {
        Upgrader upgrader = UpgraderInit(basePoint);
        Assert.Catch<ArgumentException>(() => upgrader.AddSkillPoint(skillPoint));
    }

    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void Upgrader_RemoveSkillPoint_SkillPointDecreased(int basePoint, int skillPoint)
    {
        Upgrader upgrader = UpgraderInit(basePoint);
        upgrader.RemoveSkillPoint(skillPoint);
        Assert.AreEqual(Arithmetic.IntSubtraction(basePoint, skillPoint), upgrader.SkillPoint);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void Upgrader_RemoveSkillPoint_SkillPointInvalid(int basePoint, int skillPoint)
    {
        Upgrader upgrader = UpgraderInit(basePoint);
        Assert.Catch<ArgumentException>(() => upgrader.RemoveSkillPoint(skillPoint));
    }

    private Upgrader UpgraderInit(int skillPoint)
    {
        Upgrader upgrader = new(skillPoint);
        return upgrader;
    }
}
