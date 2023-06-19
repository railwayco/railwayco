using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.Fields)]
public abstract class Helper : ICloneable
{
    protected HashSet<Guid> Collection { get; set; }

    public void Add(Guid guid) => Collection.Add(guid);
    public void Remove(Guid guid) => Collection.Remove(guid);
    public void RemoveRange(HashSet<Guid> guids)
    {
        Collection.RemoveWhere((guid) => guids.Contains(guid));
    }
    public HashSet<Guid> GetAll() => new(Collection);
    public object Clone()
    {
        Helper helper = (Helper)this.MemberwiseClone();
        helper.Collection = new(helper.Collection, helper.Collection.Comparer);
        return helper;
    }
}
