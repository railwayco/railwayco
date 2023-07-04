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
}
