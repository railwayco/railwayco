public class Arithmetic
{
    public static int IntAddition(int x, int y)
    {
        if (x > 0 && y > int.MaxValue - x)
            return int.MaxValue;
        if (x < 0 && y < int.MinValue - x)
            return int.MinValue;
        return x + y;
    }

    public static int IntSubtraction(int x, int y)
    {
        if (x < 0 && y > int.MaxValue + x)
            return int.MinValue;
        if (x > 0 && y < int.MinValue + x)
            return int.MaxValue;
        return x - y;
    }

    public static double DoubleRangeCheck(double valueToCheck)
    {
        if (double.IsPositiveInfinity(valueToCheck))
            return double.MaxValue;
        if (double.IsNegativeInfinity(valueToCheck))
            return double.MinValue;
        return valueToCheck;
    }
}