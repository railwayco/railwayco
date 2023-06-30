public abstract class Attribute<T> : Arithmetic
{
    public T LowerLimit { get; set; }
    public T UpperLimit { get; set; }
    public T Rate { get; set; }
    public T Amount { get; set; }

    public abstract void UpgradeLimit(T upgradeAmount);
    public abstract void UpgradeRate(T upgradeAmount);
}
