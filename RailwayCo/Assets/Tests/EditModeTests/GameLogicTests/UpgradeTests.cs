using System;
using NUnit.Framework;

public class UpgradeTests
{
    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void User_AddSkillPoint_SkillPointIncreased(int basePoint, int skillPoint)
    {
        Upgrade upgrade = UpgradeInit(basePoint);
        upgrade.AddSkillPoint(skillPoint);
        Assert.AreEqual(upgrade.IntAddition(basePoint, skillPoint), upgrade.SkillPoint);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void User_AddSkillPoint_SkillPointInvalid(int basePoint, int skillPoint)
    {
        Upgrade upgrade = UpgradeInit(basePoint);
        Assert.Catch<ArgumentException>(() => upgrade.AddSkillPoint(skillPoint));
    }

    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void User_RemoveSkillPoint_SkillPointDecreased(int basePoint, int skillPoint)
    {
        Upgrade upgrade = UpgradeInit(basePoint);
        upgrade.RemoveSkillPoint(skillPoint);
        Assert.AreEqual(upgrade.IntSubtraction(basePoint, skillPoint), upgrade.SkillPoint);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void User_RemoveSkillPoint_SkillPointInvalid(int basePoint, int skillPoint)
    {
        Upgrade upgrade = UpgradeInit(basePoint);
        Assert.Catch<ArgumentException>(() => upgrade.RemoveSkillPoint(skillPoint));
    }

    private Upgrade UpgradeInit(int skillPoint)
    {
        Upgrade upgrade = new(skillPoint);
        return upgrade;
    }
}
