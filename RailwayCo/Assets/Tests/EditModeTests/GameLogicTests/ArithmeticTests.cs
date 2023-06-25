using NUnit.Framework;

public class ArithmeticTests
{
    [TestCase(10, 5, ExpectedResult = 15)]
    [TestCase(int.MaxValue, 5, ExpectedResult = int.MaxValue)]
    [TestCase(int.MinValue, -5, ExpectedResult = int.MinValue)]
    public int Arithmetic_IntAddition_CorrectHandling(int baseValue, int increment)
    {
        Arithmetic arithmetic = ArithmeticInit();
        return arithmetic.IntAddition(baseValue, increment);
    }

    [TestCase(10, 5, ExpectedResult = 5)]
    [TestCase(int.MaxValue, -5, ExpectedResult = int.MaxValue)]
    [TestCase(int.MinValue, 5, ExpectedResult = int.MinValue)]
    public int Arithmetic_IntSubtraction_CorrectHandling(int baseValue, int increment)
    {
        Arithmetic arithmetic = ArithmeticInit();
        return arithmetic.IntSubtraction(baseValue, increment);
    }

    [TestCase(10.0, 5.0, ExpectedResult = 15)]
    [TestCase(double.MaxValue, double.MaxValue, ExpectedResult = double.MaxValue)]
    [TestCase(double.MinValue, double.MinValue, ExpectedResult = double.MinValue)]
    public double Arithmetic_DoubleRangeCheck_CorrectHandling(double baseValue, double increment)
    {
        Arithmetic arithmetic = ArithmeticInit();
        return arithmetic.DoubleRangeCheck(baseValue + increment);
    }

    private Arithmetic ArithmeticInit() => new();
}