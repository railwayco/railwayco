using System;
using System.Collections.Generic;

public abstract class Master<T>
{
    protected Dictionary<Guid, T> Collection { get; set; }

    protected void Add(Guid guid, T item) => Collection.Add(guid, item);

    protected void Remove(Guid guid) => Collection.Remove(guid);

    protected T Get(Guid guid)
    {
        if (!Collection.ContainsKey(guid)) return default;
        return Collection[guid];
    }
}
