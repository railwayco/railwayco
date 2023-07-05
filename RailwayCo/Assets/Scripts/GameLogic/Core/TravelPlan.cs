using System;
using System.Threading;

public class TravelPlan
{
    private readonly ReaderWriterLock _readerWriterLock = new();

    private Guid _sourceStation;
    private Guid _destinationStation;

    public TravelPlan(Guid sourceStation, Guid destinationStation)
    {
        _sourceStation = sourceStation;
        _destinationStation = destinationStation;
    }

    public Guid GetSourceStation()
    {
        _readerWriterLock.AcquireReaderLock(Timeout.Infinite);
        Guid sourceStation = _sourceStation;
        _readerWriterLock.ReleaseReaderLock();
        return sourceStation;
    }

    public Guid GetDestinationStation()
    {
        _readerWriterLock.AcquireReaderLock(Timeout.Infinite);
        Guid destinationStation = _destinationStation;
        _readerWriterLock.ReleaseReaderLock();
        return destinationStation;
    }

    public void UpdateTravelPlan(Guid sourceStation, Guid destinationStation)
    {
        _readerWriterLock.AcquireWriterLock(Timeout.Infinite);
        SetSourceStation(sourceStation);
        SetDestinationStation(destinationStation);
        _readerWriterLock.ReleaseWriterLock();
    }

    public void SetSourceStation(Guid station)
    {
        _readerWriterLock.AcquireWriterLock(Timeout.Infinite);
        _sourceStation = station;
        _readerWriterLock.ReleaseWriterLock();
    }

    public void SetDestinationStation(Guid station)
    {
        _readerWriterLock.AcquireWriterLock(Timeout.Infinite);
        _destinationStation = station;
        _readerWriterLock.ReleaseWriterLock();
    }

    public bool HasArrived(Guid station)
    {
        _readerWriterLock.AcquireReaderLock(Timeout.Infinite);
        bool outcome = _destinationStation == station;
        _readerWriterLock.ReleaseReaderLock();
        return outcome;
    }

    public bool IsAtSource(Guid station)
    {
        _readerWriterLock.AcquireReaderLock(Timeout.Infinite);
        bool outcome = _sourceStation == station;
        _readerWriterLock.ReleaseReaderLock();
        return outcome;
    }
}
