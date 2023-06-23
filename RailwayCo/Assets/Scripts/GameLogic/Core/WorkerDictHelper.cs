using System;

public class WorkerDictHelper<T> : DictHelper<T> where T : Worker
{
    public WorkerDictHelper() => Collection = new();

    public void Add(T item) => Collection.Add(item.Guid, item);

    public T GetRef(Guid guid) => (T)GetObject(guid).Clone();
}
