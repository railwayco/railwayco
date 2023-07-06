using System;

public class TravelPlan : ICloneable, IEquatable<TravelPlan>
{
    public Guid SourceStation { get; }
    public Guid DestinationStation { get; }

    public TravelPlan(Guid sourceStation, Guid destinationStation)
    {
        SourceStation = sourceStation;
        DestinationStation = destinationStation;
    }

    public bool HasArrived(Guid station) => DestinationStation == station;

    public bool IsAtSource(Guid station) => SourceStation == station;

    public object Clone() => new TravelPlan(SourceStation, DestinationStation);

    public bool Equals(TravelPlan other)
    {
        return SourceStation.Equals(other.SourceStation)
            && DestinationStation.Equals(other.DestinationStation);
    }
}
