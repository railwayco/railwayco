public class Point
{
    private int pointValue;

    public int PointValue { get => pointValue; private set => pointValue = value; }

    public Point(int pointValue) => PointValue = pointValue;

    public void AddPointValue(int pointValue) => PointValue += pointValue;
    public void RemovePointValue(int pointValue) => PointValue -= pointValue;
}
