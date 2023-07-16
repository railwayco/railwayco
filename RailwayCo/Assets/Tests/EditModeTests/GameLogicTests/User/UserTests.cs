using System;
using NUnit.Framework;

public class UserTests
{
    [Test]
    public void User_User_IsJsonSerialisedCorrectly()
    {
        User user = UserInit(100, 100);

        string jsonString = GameDataManager.Serialize(user);
        User userToVerify = GameDataManager.Deserialize<User>(jsonString);

        Assert.AreEqual(userToVerify, user);
    }
    
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
        User user = new(userName, experiencePoint, new(skillPoint), new());
        return user;
    }
}
