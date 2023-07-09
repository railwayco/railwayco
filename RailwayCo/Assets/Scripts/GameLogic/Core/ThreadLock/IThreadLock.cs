using System.Threading;

public interface IThreadLock
{
    public ReaderWriterLock ReaderWriterLock { get; }

    void AcquireReaderLock(int milisecTimeout = Timeout.Infinite) => ReaderWriterLock.AcquireReaderLock(milisecTimeout);
    void ReleaseReaderLock() => ReaderWriterLock.ReleaseReaderLock();
    void AcquireWriterLock(int milisecTimeout = Timeout.Infinite) => ReaderWriterLock.AcquireWriterLock(milisecTimeout);
    void ReleaseWriterLock() => ReaderWriterLock.ReleaseWriterLock();
}
