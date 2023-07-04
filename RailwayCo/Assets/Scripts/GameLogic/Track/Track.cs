using System;
using Newtonsoft.Json;

public class Track
{
    public int SrcStationNum { get; }
    public int DestStationNum { get; }
    public int LineNum { get; }
    public DestDirection DestDirection { get; }

    [JsonConstructor]
    private Track(int srcStationNum, int destStationNum, int lineNum, string destDirection)
    {
        SrcStationNum = srcStationNum;
        DestStationNum = destStationNum;
        LineNum = lineNum;
        DestDirection = Enum.Parse<DestDirection>(destDirection);
    }

    public Track(int srcStationNum, int destStationNum, int lineNum, DestDirection destDirection)
    {
        SrcStationNum = srcStationNum;
        DestStationNum = destStationNum;
        LineNum = lineNum;
        DestDirection = destDirection;
    }

    public Track GetEquivalentPair()
    {
        DestDirection oppositeDirection = GetOpposite();
        return new(DestStationNum, SrcStationNum, LineNum, oppositeDirection);
    }

    private DestDirection GetOpposite()
    {
        return DestDirection switch
        {
            DestDirection.Left => DestDirection.Right,
            DestDirection.Right => DestDirection.Left,
            DestDirection.Up => DestDirection.Down,
            DestDirection.Down => DestDirection.Up,
            _ => throw new MissingMemberException($"Unknown enum member {DestDirection}"),
        };
    }
}
