﻿using System;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Unity
{
    // Source: https://github.com/DeathTBO/Unity3D-Json.Net/blob/master/Assets/Newtonsoft.Json.Unity/JsonQuaternionConverter.cs
    /// <summary>
    /// This type has an issues with serialization, because normalized/magnitude are properties.
    /// These properties return a new instance of the class, so during serialization.
    /// They cause an endless loop.
    /// This serializes only the x/y/z/w values skipping those properties.
    /// </summary>
    public class JsonQuaternionConverter : JsonConverter<Quaternion>
    {
        public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
        {
            JObject j = new() { { "x", value.x }, { "y", value.y }, { "z", value.z }, { "w", value.w } };

            j.WriteTo(writer);
        }

        //CanRead is false which means the default implementation will be used instead.
        public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override bool CanWrite => true;

        public override bool CanRead => false;
    }
}
