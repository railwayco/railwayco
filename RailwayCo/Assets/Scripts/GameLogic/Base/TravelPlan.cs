using System;

public class TravelPlan
{
    public Guid SourceStationGuid { get; private set; }
    public Guid DestinationStationGuid { get; private set; }

    public TravelPlan(Guid sourceStationGuid, Guid destinationStationGuid)
    {
        SourceStationGuid = sourceStationGuid;
        DestinationStationGuid = destinationStationGuid;
    }

    public void SetSourceStation(Guid stationGuid) => SourceStationGuid = stationGuid;
    public void SetDestinationStation(Guid stationGuid) => DestinationStationGuid = stationGuid;
    public bool HasArrived(Guid stationGuid) => DestinationStationGuid == stationGuid;
    public bool IsAtSource(Guid stationGuid) => SourceStationGuid == stationGuid;
}
