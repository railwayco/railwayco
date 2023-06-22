using System;
using System.Collections.Generic;

public class DictHelper<T>
{
    public Dictionary<Guid, T> Collection { get; protected set; }

    public DictHelper() => Collection = new();

    public void Add(Guid guid, T item) => Collection.Add(guid, item);

    public void Remove(Guid guid) => Collection.Remove(guid);

    public T GetObject(Guid guid)
    {
        if (!Collection.ContainsKey(guid)) return default;
        return Collection[guid];
    }

    public HashSet<Guid> GetAllGuids() => new(Collection.Keys); 
}
