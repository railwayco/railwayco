using System.Collections;
using System.Collections.Generic;

public class Attribute<T>
{
    private T lowerLimit;
    private T upperLimit;
    private T amount;
    private T rate;

    public T LowerLimit { get => lowerLimit; set => lowerLimit = value; }
    public T UpperLimit { get => upperLimit; set => upperLimit = value; }
    public T Rate { get => rate; set => rate = value; }
    public T Amount { get => amount; set => amount = value; }

    public Attribute(T lowerLimit, T upperLimit, T amount, T rate)
    {
        this.LowerLimit = lowerLimit;
        this.UpperLimit = upperLimit;
        this.Amount = amount;
        this.Rate = rate;
    }
}
