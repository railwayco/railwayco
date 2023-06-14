using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UserTests
{
    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void User_AddExperiencePoint_ExperiencePointIncreased(int basePoint, int experiencePoint)
    {
        User user = InitObject(basePoint);
        user.AddExperiencePoint(experiencePoint);
        Assert.AreEqual(user.IntAddition(basePoint, experiencePoint), user.ExperiencePoint);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void User_AddExperiencePoint_ExperiencePointInvalid(int basePoint, int experiencePoint)
    {
        User user = InitObject(basePoint);
        Assert.Catch<ArgumentException>(() => user.AddExperiencePoint(experiencePoint));
    }

    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void User_AddSkillPoint_SkillPointIncreased(int basePoint, int skillPoint)
    {
        User user = InitObject(0, basePoint);
        user.AddSkillPoint(skillPoint);
        Assert.AreEqual(user.IntAddition(basePoint, skillPoint), user.SkillPoint);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void User_AddSkillPoint_SkillPointInvalid(int basePoint, int skillPoint)
    {
        User user = InitObject(0, basePoint);
        Assert.Catch<ArgumentException>(() => user.AddSkillPoint(skillPoint));
    }

    [TestCase(0, 50)]
    [TestCase(int.MaxValue, 1)]
    public void User_RemoveSkillPoint_SkillPointDecreased(int basePoint, int skillPoint)
    {
        User user = InitObject(0, basePoint);
        user.RemoveSkillPoint(skillPoint);
        Assert.AreEqual(user.IntSubtraction(basePoint, skillPoint), user.SkillPoint);
    }

    [TestCase(0, -50)]
    [TestCase(int.MinValue, -1)]
    public void User_RemoveSkillPoint_SkillPointInvalid(int basePoint, int skillPoint)
    {
        User user = InitObject(0, basePoint);
        Assert.Catch<ArgumentException>(() => user.RemoveSkillPoint(skillPoint));
    }

    public User InitObject(int experiencePoint = 0, int skillPoint = 0)
    {
        string userName = "TestUser";
        User user = new(userName, experiencePoint, skillPoint);
        return user;
    }
}
