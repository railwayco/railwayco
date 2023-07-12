using System;
using Newtonsoft.Json;

public class User
{
    public string Name { get; private set; }
    public int ExperiencePoint { get; private set; }
    public Upgrader Upgrader { get; private set; }

    [JsonProperty]
    private CurrencyManager CurrencyManager { get; set; }

    public User(string name, int experiencePoint, Upgrader upgrader, CurrencyManager currencyManager)
    {
        Name = name;
        ExperiencePoint = experiencePoint;
        Upgrader = upgrader;
        CurrencyManager = currencyManager;
    }

    public void AddExperiencePoint(int experiencePoint)
    {
        if (experiencePoint < 0) throw new ArgumentException("Invalid experience points");
        ExperiencePoint = Arithmetic.IntAddition(ExperiencePoint, experiencePoint);

        // TODO: Add exp calculation for level determination
        // TODO: Determine how much skill points to award after levelling up
    }

    public void AddCurrencyManager(CurrencyManager currencyManager)
    {
        CurrencyManager.AddCurrencyManager(currencyManager);
    }

    public CurrencyManager GetCurrencyManager() => (CurrencyManager)CurrencyManager.Clone();
}
