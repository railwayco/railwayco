public class User
{
    public string Name { get; private set; }
    public int ExperiencePoint { get; private set; }
    public int SkillPoint { get; private set; }
    public CurrencyManager CurrencyManager { get; private set; }

    public User(string name, int experiencePoint, int skillPoint, CurrencyManager currencyManager)
    {
        Name = name;
        ExperiencePoint = experiencePoint;
        SkillPoint = skillPoint;
        CurrencyManager = currencyManager;
    }

    public void AddExperiencePoint(int experiencePoint)
    {
        if (experiencePoint < 0) throw new System.ArgumentException("Invalid experience points");
        ExperiencePoint = Arithmetic.IntAddition(ExperiencePoint, experiencePoint);
    }

    public void AddSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new System.ArgumentException("Invalid skill points");
        SkillPoint = Arithmetic.IntAddition(SkillPoint, skillPoint);
    }

    public void RemoveSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new System.ArgumentException("Invalid skill points");
        SkillPoint = Arithmetic.IntSubtraction(SkillPoint, skillPoint);
    }
}
