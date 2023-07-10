using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.Fields)]
public class HashsetHelper : ICloneable, IEquatable<HashsetHelper>
{
    private HashSet<Guid> _collection;
    protected HashSet<Guid> Collection { get => _collection; set => _collection = value; }

    public HashsetHelper() => Collection = new();

    public int Count() => Collection.Count;
    public void Add(Guid guid) => Collection.Add(guid);
    public void Remove(Guid guid) => Collection.Remove(guid);
    public HashSet<Guid> GetAll() => new(Collection);
    public object Clone()
    {
        HashsetHelper helper = (HashsetHelper)MemberwiseClone();
        helper.Collection = new(helper.Collection, helper.Collection.Comparer);
        return helper;
    }

    public bool Equals(HashsetHelper other) => Collection.SetEquals(other.Collection);
}
