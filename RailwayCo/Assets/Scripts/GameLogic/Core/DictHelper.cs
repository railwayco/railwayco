using System;
using System.Collections.Generic;
using System.Threading;

public class DictHelper<T> : ICloneable
{
    protected Dictionary<Guid, T> Collection { get; set; }
    protected ReaderWriterLock ReaderWriterLock { get; set; }

    public DictHelper()
    {
        Collection = new();
        ReaderWriterLock = new();
    }

    public void AcquireReaderLock(int milisecTimeout = Timeout.Infinite) => ReaderWriterLock.AcquireReaderLock(milisecTimeout);
    public void ReleaseReaderLock() => ReaderWriterLock.ReleaseReaderLock();
    public void AcquireWriterLock(int milisecTimeout = Timeout.Infinite) => ReaderWriterLock.AcquireWriterLock(milisecTimeout);
    public void ReleaseWriterLock() => ReaderWriterLock.ReleaseWriterLock();

    public int Count()
    {
        AcquireReaderLock();
        int count = Collection.Count;
        ReleaseReaderLock();
        return count;
    }

    public void Add(Guid guid, T item)
    {
        AcquireWriterLock(Timeout.Infinite);
        Collection.Add(guid, item);
        ReleaseWriterLock();
    }

    public void Remove(Guid guid)
    {
        AcquireWriterLock(Timeout.Infinite);
        Collection.Remove(guid);
        ReleaseWriterLock();
    }

    public void Update(Guid guid, T value)
    {
        AcquireWriterLock(Timeout.Infinite);
        if (Collection.ContainsKey(guid)) Collection[guid] = value;
        ReleaseWriterLock();
    }

    /// <summary>
    /// Get the raw object based on requested guid. Need to AcquireWriterLock()
    /// from ReaderWriterLock and ReleaseWriterLock() when done.
    /// </summary>
    public T GetObject(Guid guid)
    {
        if (!Collection.ContainsKey(guid)) return default;
        return Collection[guid];
    }

    public HashSet<Guid> GetAll()
    {
        AcquireReaderLock(Timeout.Infinite);
        HashSet<Guid> guids = new(Collection.Keys);
        ReleaseReaderLock();
        return guids;
    }

    /// <summary>Clones Collection, given that T is of value type. If T is of reference type, 
    /// override this method and write a new implementation of Clone.</summary>
    public virtual object Clone()
    {
        AcquireReaderLock(Timeout.Infinite);
        DictHelper<T> dictHelper = (DictHelper<T>)MemberwiseClone();
        ReleaseReaderLock();

        dictHelper.Collection = new(dictHelper.Collection);

        return dictHelper;
    }
}
