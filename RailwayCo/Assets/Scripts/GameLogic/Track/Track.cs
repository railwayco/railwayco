using System;
using Newtonsoft.Json;

public class Track : IEquatable<Track>
{
    public Guid Platform { get; }
    public DepartDirection DepartDirection { get; }
    public OperationalStatus Status { get; private set; }

    [JsonConstructor]
    private Track(string platform, string departDirection, string status)
    {
        Platform = new(platform);
        DepartDirection = Enum.Parse<DepartDirection>(departDirection);
        Status = Enum.Parse<OperationalStatus>(status);
    }

    /// <summary>
    /// Creation of virtual track to link 2 platforms together
    /// </summary>
    /// <param name="platform">Destination platform</param>
    /// <param name="departDirection">Depart direction of source platform</param>
    /// <param name="status">Operational status of track</param>
    public Track(Guid platform, DepartDirection departDirection, OperationalStatus status)
    {
        Platform = platform;
        DepartDirection = departDirection;
        Status = status;
    }

    public void Open() => Status = OperationalStatus.Open;
    public void Close() => Status = OperationalStatus.Closed;
    public void Lock() => Status = OperationalStatus.Locked;
    public void Unlock() => Open();

    public bool Equals(Track other)
    {
        return Platform == other.Platform
            && DepartDirection == other.DepartDirection
            && Status == other.Status;
    }
}
