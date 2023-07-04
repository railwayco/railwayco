using System;
using System.Threading;

public class WorkerDictHelper<T> : DictHelper<T> where T : Worker
{
    public WorkerDictHelper()
    {
        Collection = new();
        ReaderWriterLock = new();
    }

    public void Add(T item)
    {
        AcquireWriterLock(Timeout.Infinite);
        Collection.Add(item.Guid, item);
        ReleaseWriterLock();
    }

    public T GetRef(Guid guid)
    {
        AcquireReaderLock(Timeout.Infinite);
        T tObject = (T)GetObject(guid).Clone();
        ReleaseReaderLock();
        return tObject;
    }
}
