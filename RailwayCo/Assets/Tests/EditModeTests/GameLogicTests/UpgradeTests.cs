using System;
using NUnit.Framework;

public class UpgradeTests
{
    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void Upgrade_AddSkillPoint_SkillPointIncreased(int basePoint, int skillPoint)
    {
        Upgrader upgrade = UpgradeInit(basePoint);
        upgrade.AddSkillPoint(skillPoint);
        Assert.AreEqual(upgrade.IntAddition(basePoint, skillPoint), upgrade.SkillPoint);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void Upgrade_AddSkillPoint_SkillPointInvalid(int basePoint, int skillPoint)
    {
        Upgrader upgrade = UpgradeInit(basePoint);
        Assert.Catch<ArgumentException>(() => upgrade.AddSkillPoint(skillPoint));
    }

    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void Upgrade_RemoveSkillPoint_SkillPointDecreased(int basePoint, int skillPoint)
    {
        Upgrader upgrade = UpgradeInit(basePoint);
        upgrade.RemoveSkillPoint(skillPoint);
        Assert.AreEqual(upgrade.IntSubtraction(basePoint, skillPoint), upgrade.SkillPoint);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void Upgrade_RemoveSkillPoint_SkillPointInvalid(int basePoint, int skillPoint)
    {
        Upgrader upgrade = UpgradeInit(basePoint);
        Assert.Catch<ArgumentException>(() => upgrade.RemoveSkillPoint(skillPoint));
    }

    private Upgrader UpgradeInit(int skillPoint)
    {
        Upgrader upgrade = new(skillPoint);
        return upgrade;
    }
}
