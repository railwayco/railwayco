public class DoubleAttribute : Attribute<double>
{
    public DoubleAttribute(double lowerLimit, double upperLimit, double amount, double rate) : base(lowerLimit, upperLimit, amount, rate) { }

    public override void UpgradeLimit() => UpperLimit = Arithmetic.DoubleRangeCheck(UpperLimit * 1.15);

    public override void UpgradeRate() => Rate = Arithmetic.DoubleRangeCheck(Rate * 1.15);
    
    public override object Clone() => new DoubleAttribute(LowerLimit, UpperLimit, Amount, Rate);
}
