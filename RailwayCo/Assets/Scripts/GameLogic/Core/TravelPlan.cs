using System;

public class TravelPlan : IEquatable<TravelPlan>
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

    public bool Equals(TravelPlan other)
    {
        return SourceStation.Equals(other.SourceStation)
            && DestinationStation.Equals(other.DestinationStation);
    }
}
