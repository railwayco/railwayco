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

    private User UserInit(int experiencePoint = 0, int skillPoint = 0)
    {
        string userName = "TestUser";
        User user = new(userName, experiencePoint, skillPoint, new());
        return user;
    }
}
