using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.Fields)]
public class HashsetHelper : ICloneable, IEquatable<HashsetHelper>, IEqualityComparer<HashsetHelper>
{
    protected HashSet<Guid> Collection { get; set; }

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

    public bool Equals(HashsetHelper other) => Equals(this, other);
    public bool Equals(HashsetHelper x, HashsetHelper y)
    {
        if (x.Collection == default || y.Collection == default)
            return false;
        return x.Collection.SetEquals(y.Collection);
    }
    public int GetHashCode(HashsetHelper obj) => obj.Collection.GetHashCode();
}
