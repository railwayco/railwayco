using System.Threading;

public class RWLock : IThreadLock
{
    private ReaderWriterLock _readerWriterLock;
    ReaderWriterLock IThreadLock.ReaderWriterLock => _readerWriterLock;

    public RWLock() => _readerWriterLock = new();
}
