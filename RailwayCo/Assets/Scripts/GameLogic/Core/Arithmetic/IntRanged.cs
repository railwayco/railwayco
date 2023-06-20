public struct IntRanged
{
    public int Value { get; set; }

    public IntRanged(int value) => Value = value;

    public static implicit operator IntRanged(int value) => new IntRanged(value);

    public static int operator +(IntRanged x, IntRanged y)
    {
        if (x.Value > 0 && y.Value > int.MaxValue - x.Value)
            return int.MaxValue;
        if (x.Value < 0 && y.Value < int.MinValue - x.Value)
            return int.MinValue;
        return x.Value + y.Value;
    }

    public static int operator -(IntRanged x, IntRanged y)
    {
        if (x.Value < 0 && y.Value > int.MaxValue + x.Value)
            return int.MinValue;
        if (x.Value > 0 && y.Value < int.MinValue + x.Value)
            return int.MaxValue;
        return x.Value - y.Value;
    }
}
