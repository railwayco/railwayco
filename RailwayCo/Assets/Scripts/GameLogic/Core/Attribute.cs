using System;

public class Attribute<T> : ICloneable
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
}
