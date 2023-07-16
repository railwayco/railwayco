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

    [Test]
    public void User_AddCurrencyManager_CurrencyManagerAdded()
    {
        User user = UserInit();
        CurrencyManager currencyManager = new();
        currencyManager.AddCurrency(new(CurrencyType.Coin, 100));
        currencyManager.AddCurrency(new(CurrencyType.Note, 50));
        currencyManager.AddCurrency(new(CurrencyType.NormalCrate, 10));
        currencyManager.AddCurrency(new(CurrencyType.SpecialCrate, 20));

        user.AddCurrencyManager(currencyManager);
        Assert.AreEqual(user.GetCurrencyManager(), currencyManager);
    }

    [Test]
    public void User_RemoveCurrency_CurrencyRemoved()
    {
        User user = UserInit();
        CurrencyManager currencyManager = new();
        currencyManager.AddCurrency(new(CurrencyType.Coin, 100));
        user.AddCurrencyManager(currencyManager);

        Currency currencyToRemove = new(CurrencyType.Coin, 50);
        user.RemoveCurrency(currencyToRemove);

        currencyManager.RemoveCurrency(currencyToRemove);
        Assert.AreEqual(user.GetCurrencyManager(), currencyManager);
    }

    private User UserInit(int experiencePoint = 0, int skillPoint = 0)
    {
        string userName = "TestUser";
        User user = new(userName, experiencePoint, new(skillPoint), new());
        return user;
    }
}
