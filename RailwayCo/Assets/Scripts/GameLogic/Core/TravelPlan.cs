using System;
using System.Threading;

public class TravelPlan
{
    public Guid SourceStation { get; set; }
    public Guid DestinationStation { get; set; }

    public TravelPlan(Guid sourceStation, Guid destinationStation)
    {
        SourceStation = sourceStation;
        DestinationStation = destinationStation;
    }

    public void UpdateTravelPlan(Guid sourceStation, Guid destinationStation)
    {
        SourceStation = sourceStation;
        DestinationStation = destinationStation;
    }

    public bool HasArrived(Guid station) => DestinationStation == station;

    public bool IsAtSource(Guid station) => SourceStation == station;
}
