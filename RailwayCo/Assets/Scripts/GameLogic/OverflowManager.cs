public class OverflowManager
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
            return int.MaxValue;
        if (baseValue > 0 && increment < int.MinValue + baseValue)
            return int.MinValue;
        return baseValue - increment;
    }
}
