public class IntAttribute : Attribute<int>
{
    public IntAttribute(int lowerLimit, int upperLimit, int amount, int rate)
    {
        LowerLimit = lowerLimit;
        UpperLimit = upperLimit;
        Amount = amount;
        Rate = rate;
    }

    public override void UpgradeLimit(int upgradeAmount) => UpperLimit = IntAddition(UpperLimit, upgradeAmount);

    public override void UpgradeRate(int upgradeAmount) => Rate = IntAddition(Rate, upgradeAmount);
}
