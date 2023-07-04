using System;
using Newtonsoft.Json;

public class Track
{
    public int SrcStationNum { get; }
    public int DestStationNum { get; }
    public int LineNum { get; }
    public DepartDirection DepartDirection { get; }

    [JsonConstructor]
    private Track(int srcStationNum, int destStationNum, int lineNum, string departDirection)
    {
        SrcStationNum = srcStationNum;
        DestStationNum = destStationNum;
        LineNum = lineNum;
        DepartDirection = Enum.Parse<DepartDirection>(departDirection);
    }

    public Track(int srcStationNum, int destStationNum, int lineNum, DepartDirection departDirection)
    {
        SrcStationNum = srcStationNum;
        DestStationNum = destStationNum;
        LineNum = lineNum;
        DepartDirection = departDirection;
    }

    public Track GetEquivalentPair()
    {
        DepartDirection oppositeDirection = GetOpposite();
        return new(DestStationNum, SrcStationNum, LineNum, oppositeDirection);
    }

    private DepartDirection GetOpposite()
    {
        return DepartDirection switch
        {
            DepartDirection.Left => DepartDirection.Right,
            DepartDirection.Right => DepartDirection.Left,
            DepartDirection.Up => DepartDirection.Down,
            DepartDirection.Down => DepartDirection.Up,
            _ => throw new MissingMemberException($"Unknown enum member {DepartDirection}"),
        };
    }
}
