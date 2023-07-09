using System;
using System.Threading;

public class WorkerDictHelper<T> : DictHelper<T> where T : Worker
{
    public WorkerDictHelper() : base() { }

    public void Add(T item)
    {
        RWLock.AcquireWriterLock(Timeout.Infinite);
        Collection.Add(item.Guid, item);
        RWLock.ReleaseWriterLock();
    }

    public T GetRef(Guid guid)
    {
        RWLock.AcquireReaderLock(Timeout.Infinite);
        T tObject = (T)GetObject(guid).Clone();
        RWLock.ReleaseReaderLock();
        return tObject;
    }
}
