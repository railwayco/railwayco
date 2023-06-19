public class User
{
    public string UserName { get; private set; }
    public IntRanged ExperiencePoint { get; private set; }
    public IntRanged SkillPoint { get; private set; }
    public CurrencyManager CurrencyManager { get; private set; }

    public User(string userName, int experiencePoint, int skillPoint, CurrencyManager currencyManager)
    {
        UserName = userName;
        ExperiencePoint = experiencePoint;
        SkillPoint = skillPoint;
        CurrencyManager = currencyManager;
    }

    public void AddExperiencePoint(int experiencePoint)
    {
        if (experiencePoint < 0) throw new System.ArgumentException("Invalid experience points");
        ExperiencePoint += experiencePoint;
    }

    public void AddSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new System.ArgumentException("Invalid skill points");
        SkillPoint += skillPoint;
    }

    public void RemoveSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new System.ArgumentException("Invalid skill points");
        SkillPoint -= skillPoint;
    }
}
