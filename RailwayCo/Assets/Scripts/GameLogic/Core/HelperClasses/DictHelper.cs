using System;
using System.Collections.Generic;
using System.Threading;

public class DictHelper<T> : ICloneable
{
    protected Dictionary<Guid, T> Collection { get; set; }
    public IThreadLock RWLock { get; }

    public DictHelper()
    {
        Collection = new();
        RWLock = new RWLock();
    }

    public int Count()
    {
        RWLock.AcquireReaderLock();
        int count = Collection.Count;
        RWLock.ReleaseReaderLock();
        return count;
    }

    public void Add(Guid guid, T item)
    {
        RWLock.AcquireWriterLock(Timeout.Infinite);
        Collection.Add(guid, item);
        RWLock.ReleaseWriterLock();
    }

    public void Remove(Guid guid)
    {
        RWLock.AcquireWriterLock(Timeout.Infinite);
        Collection.Remove(guid);
        RWLock.ReleaseWriterLock();
    }

    public void Update(Guid guid, T value)
    {
        RWLock.AcquireWriterLock(Timeout.Infinite);
        if (Collection.ContainsKey(guid)) Collection[guid] = value;
        RWLock.ReleaseWriterLock();
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
        RWLock.AcquireReaderLock(Timeout.Infinite);
        HashSet<Guid> guids = new(Collection.Keys);
        RWLock.ReleaseReaderLock();
        return guids;
    }

    /// <summary>Clones Collection, given that T is of value type. If T is of reference type, 
    /// override this method and write a new implementation of Clone.</summary>
    public virtual object Clone()
    {
        RWLock.AcquireReaderLock(Timeout.Infinite);
        DictHelper<T> dictHelper = (DictHelper<T>)MemberwiseClone();
        RWLock.ReleaseReaderLock();

        dictHelper.Collection = new(dictHelper.Collection);

        return dictHelper;
    }
}
