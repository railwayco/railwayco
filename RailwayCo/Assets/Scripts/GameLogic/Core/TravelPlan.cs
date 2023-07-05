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

    public Guid SourceStation
    {
        get
        {
            _readerWriterLock.AcquireReaderLock(Timeout.Infinite);
            Guid sourceStation = _sourceStation;
            _readerWriterLock.ReleaseReaderLock();
            return sourceStation;
        }

        set
        {
            _readerWriterLock.AcquireWriterLock(Timeout.Infinite);
            _sourceStation = value;
            _readerWriterLock.ReleaseWriterLock();
        }
    }

    public Guid DestinationStation
    {
        get
        {
            _readerWriterLock.AcquireReaderLock(Timeout.Infinite);
            Guid destinationStation = _destinationStation;
            _readerWriterLock.ReleaseReaderLock();
            return destinationStation;
        }

        set
        {
            _readerWriterLock.AcquireWriterLock(Timeout.Infinite);
            _destinationStation = value;
            _readerWriterLock.ReleaseWriterLock();
        }
    }

    public void UpdateTravelPlan(Guid sourceStation, Guid destinationStation)
    {
        _readerWriterLock.AcquireWriterLock(Timeout.Infinite);
        SourceStation = sourceStation;
        DestinationStation = destinationStation;
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
