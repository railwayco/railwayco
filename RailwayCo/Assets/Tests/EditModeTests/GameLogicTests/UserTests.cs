using System;
using NUnit.Framework;

public class UserTests
{
    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void User_AddExperiencePoint_ExperiencePointIncreased(int basePoint, int experiencePoint)
    {
        User user = UserInit(basePoint);
        user.AddExperiencePoint(experiencePoint);
        Assert.AreEqual(Arithmetic.IntAddition(basePoint, experiencePoint), user.ExperiencePoint);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void User_AddExperiencePoint_ExperiencePointInvalid(int basePoint, int experiencePoint)
    {
        User user = UserInit(basePoint);
        Assert.Catch<ArgumentException>(() => user.AddExperiencePoint(experiencePoint));
    }

    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void User_AddSkillPoint_SkillPointIncreased(int basePoint, int skillPoint)
    {
        User user = UserInit(0, basePoint);
        user.AddSkillPoint(skillPoint);
        Assert.AreEqual(Arithmetic.IntAddition(basePoint, skillPoint), user.SkillPoint);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void User_AddSkillPoint_SkillPointInvalid(int basePoint, int skillPoint)
    {
        User user = UserInit(0, basePoint);
        Assert.Catch<ArgumentException>(() => user.AddSkillPoint(skillPoint));
    }

    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void User_RemoveSkillPoint_SkillPointDecreased(int basePoint, int skillPoint)
    {
        User user = UserInit(0, basePoint);
        user.RemoveSkillPoint(skillPoint);
        Assert.AreEqual(Arithmetic.IntSubtraction(basePoint, skillPoint), user.SkillPoint);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void User_RemoveSkillPoint_SkillPointInvalid(int basePoint, int skillPoint)
    {
        User user = UserInit(0, basePoint);
        Assert.Catch<ArgumentException>(() => user.RemoveSkillPoint(skillPoint));
    }

    private User UserInit(int experiencePoint = 0, int skillPoint = 0)
    {
        string userName = "TestUser";
        User user = new(userName, experiencePoint, skillPoint, new());
        return user;
    }
}
