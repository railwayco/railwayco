using System;
using System.Collections.Generic;

public static class WorkerDictionaryExtensions
{
    public static void Add<T>(this Dictionary<Guid, T> dictionary, T item) where T : Worker
    {
        dictionary.Add(item.Guid, item);
    }

    public static T GetObject<T>(this Dictionary<Guid, T> dictionary, Guid guid) where T : Worker
    {
        return dictionary.GetValueOrDefault(guid);
    }

    public static T GetRef<T>(this Dictionary<Guid, T> dictionary, Guid guid) where T : Worker
    {
        T worker = dictionary.GetObject(guid);
        if (worker == default)
            return default;
        return (T)worker.Clone();
    }
}
