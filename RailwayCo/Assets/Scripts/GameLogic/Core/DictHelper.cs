using System;
using System.Collections.Generic;

public class DictHelper<T> : ICloneable
{
    protected Dictionary<Guid, T> Collection { get; set; }

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
