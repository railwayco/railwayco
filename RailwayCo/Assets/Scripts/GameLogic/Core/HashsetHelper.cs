using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.Fields)]
public class HashsetHelper : ICloneable
{
    protected HashSet<Guid> Collection { get; set; }

    public HashsetHelper() => Collection = new();

    public void Add(Guid guid) => Collection.Add(guid);
    public void Remove(Guid guid) => Collection.Remove(guid);
    public HashSet<Guid> GetAll() => new(Collection);
    public object Clone()
    {
        HashsetHelper helper = (HashsetHelper)MemberwiseClone();
        helper.Collection = new(helper.Collection, helper.Collection.Comparer);
        return helper;
    }
}
