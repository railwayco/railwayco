using System;
using System.Threading;

public class User
{
    public IThreadLock RWLock { get; }
    private CurrencyManager _currencyManager;
    private string _name;
    private int _experiencePoint;
    private int _skillPoint;

    public string Name 
    {
        get 
        {
            RWLock.AcquireReaderLock();
            string name = _name;
            RWLock.ReleaseReaderLock();
            return name;
        }
        private set
        {
            RWLock.AcquireWriterLock();
            _name = value;
            RWLock.ReleaseWriterLock();
        } 
    }
    public int ExperiencePoint
    {
        get
        {
            RWLock.AcquireReaderLock();
            int experiencePoint = _experiencePoint;
            RWLock.ReleaseReaderLock();
            return experiencePoint;
        }
        private set
        {
            RWLock.AcquireWriterLock();
            _experiencePoint = value;
            RWLock.ReleaseWriterLock();
        }
    }
    public int SkillPoint
    {
        get
        {
            RWLock.AcquireReaderLock();
            int skillPoint = _skillPoint;
            RWLock.ReleaseReaderLock();
            return skillPoint;
        }
        private set
        {
            RWLock.AcquireWriterLock();
            _skillPoint = value;
            RWLock.ReleaseWriterLock();
        }
    }
    public CurrencyManager CurrencyManager
    {
        get
        {
            RWLock.AcquireReaderLock();
            CurrencyManager currencyManager = (CurrencyManager)_currencyManager.Clone();
            RWLock.ReleaseReaderLock();
            return currencyManager;
        }
        private set
        {
            RWLock.AcquireWriterLock();
            _currencyManager = value;
            RWLock.ReleaseWriterLock();
        }
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
        ExperiencePoint = Arithmetic.IntAddition(ExperiencePoint, experiencePoint);
    }

    public void AddSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new ArgumentException("Invalid skill points");
        SkillPoint = Arithmetic.IntAddition(SkillPoint, skillPoint);
    }

    public void RemoveSkillPoint(int skillPoint)
    {
        if (skillPoint < 0) throw new ArgumentException("Invalid skill points");
        SkillPoint = Arithmetic.IntSubtraction(SkillPoint, skillPoint);
    }

    public void AddCurrencyManager(CurrencyManager currencyManager)
    {
        RWLock.AcquireWriterLock();
        CurrencyManager.AddCurrencyManager(currencyManager);
        RWLock.ReleaseWriterLock();
    }
}
