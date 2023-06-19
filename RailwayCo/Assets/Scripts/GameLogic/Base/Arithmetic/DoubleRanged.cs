using System;

public struct DoubleRanged
{
    public double Value { get; set; }

    public DoubleRanged(double value) => Value = value;

    public static implicit operator DoubleRanged(double value) => new(value);
    public static explicit operator double(DoubleRanged value) => value.Value;

    public static double operator +(DoubleRanged x, DoubleRanged y) => RangeCheck(x.Value + y.Value);
    public static double operator -(DoubleRanged x, DoubleRanged y) => RangeCheck(x.Value - y.Value);
    public static double operator *(DoubleRanged x, DoubleRanged y) => RangeCheck(x.Value * y.Value);
    public static double operator /(DoubleRanged x, DoubleRanged y) => RangeCheck(x.Value / y.Value);

    private static double RangeCheck(double valueToCheck)
    {
        if (double.IsPositiveInfinity(valueToCheck))
            return double.MaxValue;
        if (double.IsNegativeInfinity(valueToCheck))
            return double.MinValue;
        return valueToCheck;
    }
}
