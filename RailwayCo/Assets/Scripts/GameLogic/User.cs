using System;
using System.Threading;

public class User
{
    private ReaderWriterLock _readerWriterLock = new();
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
        Name = name;
        ExperiencePoint = experiencePoint;
        SkillPoint = skillPoint;
        CurrencyManager = currencyManager;
    }

    public void AddExperiencePoint(int experiencePoint)
    {
        if (experiencePoint < 0) throw new ArgumentException("Invalid experience points");
        _readerWriterLock.AcquireWriterLock(Timeout.Infinite);
        ExperiencePoint = Arithmetic.IntAddition(ExperiencePoint, experiencePoint);
        _readerWriterLock.ReleaseWriterLock();
    }

    public void AddSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new ArgumentException("Invalid skill points");
        _readerWriterLock.AcquireWriterLock(Timeout.Infinite);
        SkillPoint = Arithmetic.IntAddition(SkillPoint, skillPoint);
        _readerWriterLock.ReleaseWriterLock();
    }

    public void RemoveSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new ArgumentException("Invalid skill points");
        _readerWriterLock.AcquireWriterLock(Timeout.Infinite);
        SkillPoint = Arithmetic.IntSubtraction(SkillPoint, skillPoint);
        _readerWriterLock.ReleaseWriterLock();
    }

    public void AddCurrencyManager(CurrencyManager currencyManager)
    {
        _readerWriterLock.AcquireWriterLock(Timeout.Infinite);
        CurrencyManager.AddCurrencyManager(currencyManager);
        _readerWriterLock.ReleaseWriterLock();
    }
}
