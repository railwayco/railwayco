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

    public void AddExperience(int pointValue) => Experience.AddPointValue(pointValue);
    public void AddSkill(int pointValue) => Skill.AddPointValue(pointValue);
    public void RemoveSkill(int pointValue) => Skill.RemovePointValue(pointValue);
    public void AddCurrencyList(CurrencyManager currencyManager) => CurrencyManager.AddCurrencyManager(currencyManager);
    public void RemoveCurrencyList(CurrencyManager currencyManager) => CurrencyManager.RemoveCurrencyManager(currencyManager);
}
