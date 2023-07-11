public class IntAttribute : Attribute<int>
{
    public IntAttribute(int lowerLimit, int upperLimit, int amount, int rate) : base(lowerLimit, upperLimit, amount, rate) { }

    public override void UpgradeLimit(int upgradeAmount) => UpperLimit = Arithmetic.IntAddition(UpperLimit, upgradeAmount);

    public override void UpgradeRate(int upgradeAmount) => Rate = Arithmetic.IntAddition(Rate, upgradeAmount);

    public override object Clone() => new IntAttribute(LowerLimit, UpperLimit, Amount, Rate);
}
