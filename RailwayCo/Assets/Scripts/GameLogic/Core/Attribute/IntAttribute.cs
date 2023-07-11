public class IntAttribute : Attribute<int>
{
    public IntAttribute(int lowerLimit, int upperLimit, int amount, int rate) : base(lowerLimit, upperLimit, amount, rate) { }

    public override void UpgradeLimit() => UpperLimit = Arithmetic.IntAddition(UpperLimit, (int)(UpperLimit * 0.15));

    public override void UpgradeRate() => Rate = Arithmetic.IntAddition(Rate, (int)(Rate * 0.15));

    public override object Clone() => new IntAttribute(LowerLimit, UpperLimit, Amount, Rate);
}
