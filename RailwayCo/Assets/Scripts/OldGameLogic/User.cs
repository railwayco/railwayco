public class User : OverflowManager
{
    private string userName;
    private int experiencePoint;
    private int skillPoint;
    private CurrencyManager currencyManager;

    public string UserName { get => userName; private set => userName = value; }
    public int ExperiencePoint { get => experiencePoint; private set => experiencePoint = value; }
    public int SkillPoint { get => skillPoint; private set => skillPoint = value; }
    public CurrencyManager CurrencyManager { get => currencyManager; private set => currencyManager = value; }

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
        ExperiencePoint = IntAddition(ExperiencePoint, experiencePoint);
    }

    public void AddSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new System.ArgumentException("Invalid skill points");
        SkillPoint = IntAddition(SkillPoint, skillPoint);
    }

    public void RemoveSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new System.ArgumentException("Invalid skill points");
        SkillPoint = IntSubtraction(SkillPoint, skillPoint);
    }
}
