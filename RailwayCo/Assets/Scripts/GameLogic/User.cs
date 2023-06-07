public class User
{
    private string userName;
    private int experiencePoint;
    private int skillPoint;
    private CurrencyManager currencyManager;

    public string UserName { get => userName; private set => userName = value; }
    public int ExperiencePoint { get => experiencePoint; private set => experiencePoint = value; }
    public int SkillPoint { get => skillPoint; private set => skillPoint = value; }
    public CurrencyManager CurrencyManager { get => currencyManager; private set => currencyManager = value; }

    public void AddExperiencePoint(int experiencePoint) => ExperiencePoint += experiencePoint;
    public void AddSkillPoint(int skillPoint) => SkillPoint += skillPoint;
    public void RemoveSkillPoint(int skillPoint) => SkillPoint += skillPoint;
}
