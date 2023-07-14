using System;
using Newtonsoft.Json;

public class Track : ICloneable, IEquatable<Track>
{
    public Guid Platform { get; }
    public OperationalStatus Status { get; private set; }

    [JsonConstructor]
    private Track(string platform, string status)
    {
        Platform = new(platform);
        Status = Enum.Parse<OperationalStatus>(status);
    }

    /// <summary>
    /// Creation of virtual track to link 2 platforms together
    /// </summary>
    /// <param name="platform">Destination platform</param>
    /// <param name="status">Operational status of track</param>
    public Track(Guid platform, OperationalStatus status)
    {
        Platform = platform;
        Status = status;
    }

    public void Open() => Status = OperationalStatus.Open;
    public void Close() => Status = OperationalStatus.Closed;
    public void Lock() => Status = OperationalStatus.Locked;
    public void Unlock() => Open();

    public object Clone() => new Track(Platform, Status);

    public bool Equals(Track other)
    {
        return Platform == other.Platform
            && Status == other.Status;
    }
}
