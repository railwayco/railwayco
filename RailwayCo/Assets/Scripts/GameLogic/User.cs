using System;
using System.Threading;

public class User
{
    public IThreadLock RWLock { get; }
    private CurrencyManager _currencyManager;

    public string Name { get; private set; }
    public int ExperiencePoint { get; private set; }
    public int SkillPoint { get; private set; }
    public CurrencyManager CurrencyManager 
    { 
        get => (CurrencyManager)_currencyManager.Clone();
        private set => _currencyManager = value;
    }

    public User(string name, int experiencePoint, int skillPoint, CurrencyManager currencyManager)
    {
        RWLock = new RWLock();

        Name = name;
        ExperiencePoint = experiencePoint;
        SkillPoint = skillPoint;
        CurrencyManager = currencyManager;
    }

    public void AddExperiencePoint(int experiencePoint)
    {
        if (experiencePoint < 0) throw new ArgumentException("Invalid experience points");
        RWLock.AcquireWriterLock(Timeout.Infinite);
        ExperiencePoint = Arithmetic.IntAddition(ExperiencePoint, experiencePoint);
        RWLock.ReleaseWriterLock();
    }

    public void AddSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new ArgumentException("Invalid skill points");
        RWLock.AcquireWriterLock(Timeout.Infinite);
        SkillPoint = Arithmetic.IntAddition(SkillPoint, skillPoint);
        RWLock.ReleaseWriterLock();
    }

    public void RemoveSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new ArgumentException("Invalid skill points");
        RWLock.AcquireWriterLock(Timeout.Infinite);
        SkillPoint = Arithmetic.IntSubtraction(SkillPoint, skillPoint);
        RWLock.ReleaseWriterLock();
    }

    public void AddCurrencyManager(CurrencyManager currencyManager)
    {
        RWLock.AcquireWriterLock(Timeout.Infinite);
        CurrencyManager.AddCurrencyManager(currencyManager);
        RWLock.ReleaseWriterLock();
    }
}
