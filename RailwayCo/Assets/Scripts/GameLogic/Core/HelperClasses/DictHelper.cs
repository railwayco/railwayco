using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.Fields)]
public class DictHelper<T> : ICloneable
{
    private Dictionary<Guid, T> _collection;
    protected Dictionary<Guid, T> Collection { get => _collection; set => _collection = value; }

    public DictHelper() => Collection = new();

    public int Count() => Collection.Count;

    public void Add(Guid guid, T item) => Collection.Add(guid, item);

    public void Remove(Guid guid) => Collection.Remove(guid);

    public void Update(Guid guid, T value)
    {
        if (Collection.ContainsKey(guid)) Collection[guid] = value;
    }

    public T GetObject(Guid guid)
    {
        if (!Collection.ContainsKey(guid)) return default;
        return Collection[guid];
    }

    public HashSet<Guid> GetAll() => new(Collection.Keys);

    /// <summary>Clones Collection, given that T is of value type. If T is of reference type, 
    /// override this method and write a new implementation of Clone.</summary>
    public virtual object Clone()
    {
        DictHelper<T> dictHelper = (DictHelper<T>)MemberwiseClone();

        dictHelper.Collection = new(dictHelper.Collection);

        return dictHelper;
    }
}
