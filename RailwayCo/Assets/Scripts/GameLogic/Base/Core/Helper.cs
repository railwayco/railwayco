using System;
using System.Collections.Generic;

public class Helper
{
    protected HashSet<Guid> Collection { get; set; }

    public void Add(Guid guid) => Collection.Add(guid);
    public void Remove(Guid guid) => Collection.Remove(guid);
    public void RemoveRange(HashSet<Guid> guids)
    {
        Collection.RemoveWhere((guid) => guids.Contains(guid));
    }
    public HashSet<Guid> GetAll() => new(Collection);
}
