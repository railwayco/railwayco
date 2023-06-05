using System.Collections.Generic;

public class Point
{
    private int pointValue;
    public int PointValue { get => pointValue; private set => pointValue = value; }

    public Point(int pointValue)
    {
        PointValue = pointValue;
    }

    public Dictionary<string, string> AddPointValue(int pointValue)
    {
        Dictionary<string, string> result = new()
        {
            { "old", PointValue.ToString() }
        };

        PointValue += pointValue;
        result.Add("new", PointValue.ToString());

        return result;
    }

    public Dictionary<string, string> RemovePointValue(int pointValue)
    {
        Dictionary<string, string> result = new()
        {
            { "old", PointValue.ToString() }
        };

        PointValue -= pointValue;
        result.Add("new", PointValue.ToString());

        return result;
    }
}
