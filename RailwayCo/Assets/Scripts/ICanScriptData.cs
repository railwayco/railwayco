using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

public class ICanScriptData : MonoBehaviour
{
    public GameLogic GameLogic { get; set; }

    private void Start()
    {
        GameLogic = new();
        List<Guid> guids = GameLogic.GetAllStationGuids().ToList();
        GameLogic.AddRandomCargoToStation(guids[0], 10);

        using (StreamWriter file = File.CreateText(@"Assets/GameData.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            // serializer.Converters.Add(new StringEnumConverter());
            serializer.Serialize(file, GameLogic);
        }

        using (StreamReader file = File.OpenText(@"Assets/GameData.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            // serializer.Converters.Add(new StringEnumConverter());
            GameLogic gameLogic2 = (GameLogic)serializer.Deserialize(file, typeof(GameLogic));
        }

    }
}