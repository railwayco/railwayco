using System;
using UnityEngine;
using Newtonsoft.Json.Utilities;

internal class GameLogicAOT : MonoBehaviour
{
    private void Awake()
    {
        AotHelper.Ensure(() => new User("", 0, new(0), new()));
        AotHelper.EnsureDictionary<Guid, Cargo>();
        AotHelper.EnsureDictionary<Guid, Train>();
        AotHelper.EnsureDictionary<Guid, Station>();
        AotHelper.EnsureDictionary<Guid, Platform>();
        AotHelper.EnsureList<Guid>();
    }
}
