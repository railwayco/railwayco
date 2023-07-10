using System;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.Fields)]
public class WorkerDictHelper<T> : DictHelper<T> where T : Worker
{
    public WorkerDictHelper() : base() { }

    public void Add(T item) => Collection.Add(item.Guid, item);

    public T GetRef(Guid guid) => (T)GetObject(guid).Clone();
}
