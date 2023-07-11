using System;

public abstract class Attribute<T> : ICloneable, IEquatable<Attribute<T>>
{
    public T LowerLimit { get; protected set; }
    public T UpperLimit { get; protected set; }
    public T Rate { get; protected set; }
    public T Amount { get; set; }

    public Attribute(T lowerLimit, T upperLimit, T amount, T rate)
    {
        LowerLimit = lowerLimit;
        UpperLimit = upperLimit;
        Amount = amount;
        Rate = rate;
    }

    public abstract void UpgradeLimit(T upgradeAmount);
    public abstract void UpgradeRate(T upgradeAmount);

    public abstract object Clone();

    public bool Equals(Attribute<T> other)
    {
        return LowerLimit.Equals(other.LowerLimit)
            && UpperLimit.Equals(other.UpperLimit)
            && Amount.Equals(other.Amount)
            && Rate.Equals(other.Rate);
    }
}
