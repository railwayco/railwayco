using System;
using System.Collections.Generic;

public class Helper
{
    protected HashSet<Guid> Collection { get; set; }

    public void Add(Guid guid) => Collection.Add(guid);
    public void Remove(Guid guid) => Collection.Remove(guid);
    public HashSet<Guid> GetAll() => new(Collection);
}
