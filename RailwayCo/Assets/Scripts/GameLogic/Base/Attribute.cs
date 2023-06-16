public class Attribute<T>
{
    public T LowerLimit { get; set; }
    public T UpperLimit { get; set; }
    public T Rate { get; set; }
    public T Amount { get; set; }

    public Attribute(T lowerLimit, T upperLimit, T amount, T rate)
    {
        LowerLimit = lowerLimit;
        UpperLimit = upperLimit;
        Amount = amount;
        Rate = rate;
    }
}
