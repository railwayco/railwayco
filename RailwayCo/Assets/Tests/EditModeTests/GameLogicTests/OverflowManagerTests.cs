using NUnit.Framework;

public class OverflowManagerTests
{
    [TestCase(10, 5, ExpectedResult = 15)]
    [TestCase(int.MaxValue, 5, ExpectedResult = int.MaxValue)]
    [TestCase(int.MinValue, -5, ExpectedResult = int.MinValue)]
    public int OverflowManager_IntAddition_CorrectHandling(int baseValue, int increment)
    {
        OverflowManager overflowManager = OverflowManagerInit();
        return overflowManager.IntAddition(baseValue, increment);
    }

    [TestCase(10, 5, ExpectedResult = 5)]
    [TestCase(int.MaxValue, -5, ExpectedResult = int.MaxValue)]
    [TestCase(int.MinValue, 5, ExpectedResult = int.MinValue)]
    public int OverflowManager_IntSubtraction_CorrectHandling(int baseValue, int increment)
    {
        OverflowManager overflowManager = OverflowManagerInit();
        return overflowManager.IntSubtraction(baseValue, increment);
    }
    
    [TestCase(10.0, 5.0, ExpectedResult = 15)]
    [TestCase(double.MaxValue, double.MaxValue, ExpectedResult = double.MaxValue)]
    [TestCase(double.MinValue, double.MinValue, ExpectedResult = double.MinValue)]
    public double OverflowManager_DoubleArithmetic_CorrectHandling(double baseValue, double increment)
    {
        OverflowManager overflowManager = OverflowManagerInit();
        return overflowManager.DoubleArithmetic(baseValue + increment);
    }

    private OverflowManager OverflowManagerInit() => new();
}
