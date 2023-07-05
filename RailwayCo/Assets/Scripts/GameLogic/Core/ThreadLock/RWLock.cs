using System.Threading;

public class RWLock : IThreadLock
{
    private ReaderWriterLock _readerWriterLock;
    ReaderWriterLock IThreadLock.ReaderWriterLock 
    { 
        get => _readerWriterLock; 
        set => _readerWriterLock = value;
    }

    public RWLock() => _readerWriterLock = new();
}
