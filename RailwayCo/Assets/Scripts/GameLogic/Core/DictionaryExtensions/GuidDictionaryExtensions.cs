using System;
using System.Collections.Generic;

public static class GuidDictionaryExtensions
{
    public static HashSet<Guid> GetAllGuids<T>(this Dictionary<Guid, T> dictionary)
    {
        return new(dictionary.Keys);
    }
}
