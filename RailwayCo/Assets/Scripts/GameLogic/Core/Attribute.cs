using System;
using System.Collections.Generic;

public class Attribute<T> : ICloneable, IEquatable<Attribute<T>>
{
    public T LowerLimit { get; }
    public T UpperLimit { get; }
    public T Rate { get; }
    public T Amount { get; set; }

    public Attribute(T lowerLimit, T upperLimit, T amount, T rate)
    {
        LowerLimit = lowerLimit;
        UpperLimit = upperLimit;
        Amount = amount;
        Rate = rate;
    }

    public object Clone() => new Attribute<T>(LowerLimit, UpperLimit, Amount, Rate);

    public bool Equals(Attribute<T> other)
    {
        return LowerLimit.Equals(other.LowerLimit)
            && UpperLimit.Equals(other.UpperLimit)
            && Amount.Equals(other.Amount)
            && Rate.Equals(other.Rate);
    }
}
