public class Arithmetic
{
    public int IntAddition(int baseValue, int increment)
    {
        if (baseValue > 0 && increment > int.MaxValue - baseValue)
            return int.MaxValue;
        if (baseValue < 0 && increment < int.MinValue - baseValue)
            return int.MinValue;
        return baseValue + increment;
    }

    public int IntSubtraction(int baseValue, int increment)
    {
        if (baseValue < 0 && increment > int.MaxValue + baseValue)
            return int.MinValue;
        if (baseValue > 0 && increment < int.MinValue + baseValue)
            return int.MaxValue;
        return baseValue - increment;
    }

    public double DoubleArithmetic(double valueToCheck)
    {
        if (double.IsPositiveInfinity(valueToCheck))
            return double.MaxValue;
        if (double.IsNegativeInfinity(valueToCheck))
            return double.MinValue;
        return valueToCheck;
    }
}
