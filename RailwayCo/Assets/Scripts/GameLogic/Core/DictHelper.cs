using System;
using System.Collections.Generic;

public class DictHelper<T> where T : Worker
{
    public Dictionary<Guid, T> Collection { get; private set; }

    public void Add(Guid guid, T item) => Collection.Add(guid, item);

    public void Remove(Guid guid) => Collection.Remove(guid);

    public T GetRef(Guid guid) => (T)GetObject(guid).Clone();

    public T GetObject(Guid guid)
    {
        if (!Collection.ContainsKey(guid)) return default;
        return Collection[guid];
    }

    public HashSet<Guid> GetAll() => new(Collection.Keys);
}
