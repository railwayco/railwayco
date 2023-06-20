using System;
using System.Collections.Generic;

public class HashsetHelper : ICloneable
{
    protected HashSet<Guid> Collection { get; set; }

    public HashsetHelper(HashSet<Guid> collection) => Collection = collection;

    public void Add(Guid guid) => Collection.Add(guid);
    public void Remove(Guid guid) => Collection.Remove(guid);
    public void RemoveRange(HashSet<Guid> guids)
    {
        foreach (var guid in guids) Remove(guid);
    }
    public HashSet<Guid> GetAll() => new(Collection);
    public object Clone()
    {
        HashsetHelper helper = (HashsetHelper)this.MemberwiseClone();
        helper.Collection = new(helper.Collection, helper.Collection.Comparer);
        return helper;
    }
}
