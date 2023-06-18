using System;

public class TravelPlan
{
    public Guid SourceStation { get; private set; }
    public Guid DestinationStation { get; private set; }

    public TravelPlan(Guid sourceStation, Guid destinationStation)
    {
        SourceStation = sourceStation;
        DestinationStation = destinationStation;
    }

    public void SetSourceStation(Guid station) => SourceStation = station;
    public void SetDestinationStation(Guid station) => DestinationStation = station;
    public bool HasArrived(Guid station) => DestinationStation == station;
    public bool IsAtSource(Guid station) => SourceStation == station;
}
