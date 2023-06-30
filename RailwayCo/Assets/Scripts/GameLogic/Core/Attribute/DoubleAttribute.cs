public class DoubleAttribute : Attribute<double>
{
    public DoubleAttribute(double lowerLimit, double upperLimit, double amount, double rate)
    {
        LowerLimit = lowerLimit;
        UpperLimit = upperLimit;
        Amount = amount;
        Rate = rate;
    }

    public override void UpgradeLimit(double upgradeAmount) => UpperLimit = DoubleRangeCheck(UpperLimit + upgradeAmount);

    public override void UpgradeRate(double upgradeAmount) => Rate = DoubleRangeCheck(Rate + upgradeAmount);
}
