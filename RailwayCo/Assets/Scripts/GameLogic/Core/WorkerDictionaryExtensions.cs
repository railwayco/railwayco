using System;
using System.Collections.Generic;

namespace RailwayCo.GameLogic.DictionaryExtensions
{
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
            return (T)dictionary.GetObject(guid).Clone();
        }
    }
}
