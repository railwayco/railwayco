using System;
using System.Collections.Generic;

public class Attribute<T> : ICloneable, IEquatable<Attribute<T>>, IEqualityComparer<Attribute<T>>
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

    public bool Equals(Attribute<T> other) => Equals(this, other);
    public bool Equals(Attribute<T> x, Attribute<T> y)
    {
        if (x == default || y == default) return false;
        return x.LowerLimit.Equals(y.LowerLimit)
            && x.UpperLimit.Equals(y.UpperLimit)
            && x.Amount.Equals(y.Amount)
            && x.Rate.Equals(y.Rate);
    }
    public int GetHashCode(Attribute<T> obj) => LowerLimit.GetHashCode()
                                                ^ UpperLimit.GetHashCode()
                                                ^ Amount.GetHashCode()
                                                ^ Rate.GetHashCode();
}
