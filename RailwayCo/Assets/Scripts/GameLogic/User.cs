using System.Collections;
using System.Collections.Generic;

public class User
{
    private string userName;
    private Point experience;
    private Point skill;
    private CurrencyManager currencyManager;

    public string UserName { get => userName; private set => userName = value; }
    public Point Experience { get => experience; private set => experience = value; }
    public Point Skill { get => skill; private set => skill = value; }
    private CurrencyManager CurrencyManager { get => currencyManager; set => currencyManager = value; }

    public Dictionary<string, string> AddExperience(int pointValue)
    {
        Dictionary<string, string> result = new()
        {
            { "old", Experience.PointValue.ToString() }
        };

        Experience.AddPointValue(pointValue);
        result.Add("new", Experience.PointValue.ToString());

        return result;
    }

    public Dictionary<string, string> AddSkill(int pointValue)
    {
        Dictionary<string, string> result = new()
        {
            { "old", Skill.PointValue.ToString() }
        };

        Skill.AddPointValue(pointValue);
        result.Add("new", Skill.PointValue.ToString());

        return result;
    }

    public Dictionary<string, string> RemoveSkill(int pointValue)
    {
        Dictionary<string, string> result = new()
        {
            { "old", Skill.PointValue.ToString() }
        };

        Skill.RemovePointValue(pointValue);
        result.Add("new", Skill.PointValue.ToString());

        return result;
    }

    public void AddCurrencyList(CurrencyManager currencyManager)
    {
        CurrencyManager.AddCurrencyManager(currencyManager);
    }

    public void RemoveCurrencyList(CurrencyManager currencyManager)
    {
        CurrencyManager.RemoveCurrencyManager(currencyManager);
    }
}
