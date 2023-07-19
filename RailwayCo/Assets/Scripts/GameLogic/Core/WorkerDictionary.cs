using System;
using System.Collections.Generic;

public class WorkerDictionary<T> : Dictionary<Guid, T> where T : Worker
{
    public WorkerDictionary() : base() { }

    public void Add(T item) => Add(item.Guid, item);

    public T GetObject(Guid guid)
    {
        TryGetValue(guid, out T obj);
        return obj;
    }

    public T GetRef(Guid guid) => (T)GetObject(guid).Clone();
}
