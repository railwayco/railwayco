public class DoubleAttribute : Attribute<double>
{
    public DoubleAttribute(double lowerLimit, double upperLimit, double amount, double rate) : base(lowerLimit, upperLimit, amount, rate) { }

    public override void UpgradeLimit(double upgradeAmount) => UpperLimit = Arithmetic.DoubleRangeCheck(UpperLimit + upgradeAmount);

    public override void UpgradeRate(double upgradeAmount) => Rate = Arithmetic.DoubleRangeCheck(Rate + upgradeAmount);
    
    public override object Clone() => new DoubleAttribute(LowerLimit, UpperLimit, Amount, Rate);
}
