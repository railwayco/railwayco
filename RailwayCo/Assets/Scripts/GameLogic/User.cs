using System;
using System.Threading;

public class User
{
    private ReaderWriterLock ReaderWriterLock { get; }
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
        ReaderWriterLock = new();

        Name = name;
        ExperiencePoint = experiencePoint;
        SkillPoint = skillPoint;
        CurrencyManager = currencyManager;
    }

    public void AcquireReaderLock(int milisecTimeout = Timeout.Infinite) => ReaderWriterLock.AcquireReaderLock(milisecTimeout);
    public void ReleaseReaderLock() => ReaderWriterLock.ReleaseReaderLock();
    public void AcquireWriterLock(int milisecTimeout = Timeout.Infinite) => ReaderWriterLock.AcquireWriterLock(milisecTimeout);
    public void ReleaseWriterLock() => ReaderWriterLock.ReleaseWriterLock();

    public void AddExperiencePoint(int experiencePoint)
    {
        if (experiencePoint < 0) throw new ArgumentException("Invalid experience points");
        AcquireWriterLock(Timeout.Infinite);
        ExperiencePoint = Arithmetic.IntAddition(ExperiencePoint, experiencePoint);
        ReleaseWriterLock();
    }

    public void AddSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new ArgumentException("Invalid skill points");
        AcquireWriterLock(Timeout.Infinite);
        SkillPoint = Arithmetic.IntAddition(SkillPoint, skillPoint);
        ReleaseWriterLock();
    }

    public void RemoveSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new ArgumentException("Invalid skill points");
        AcquireWriterLock(Timeout.Infinite);
        SkillPoint = Arithmetic.IntSubtraction(SkillPoint, skillPoint);
        ReleaseWriterLock();
    }

    public void AddCurrencyManager(CurrencyManager currencyManager)
    {
        AcquireWriterLock(Timeout.Infinite);
        CurrencyManager.AddCurrencyManager(currencyManager);
        ReleaseWriterLock();
    }
}
