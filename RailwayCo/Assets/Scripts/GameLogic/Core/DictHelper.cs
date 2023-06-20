using System;
using System.Collections.Generic;

public class DictHelper<T> where T : Worker
{
    public Dictionary<Guid, T> Collection { get; private set; }

    public DictHelper() => Collection = new();

    public void Add(T item) => Collection.Add(item.Guid, item);

    public void Remove(Guid guid) => Collection.Remove(guid);

    public T GetRef(Guid guid) => (T)GetObject(guid).Clone();

    public T GetObject(Guid guid)
    {
        if (!Collection.ContainsKey(guid)) return default;
        return Collection[guid];
    }

    public HashSet<Guid> GetAllGuids() => new(Collection.Keys); 
}
