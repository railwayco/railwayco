using System;
using UnityEngine;
using Newtonsoft.Json.Linq;


namespace Newtonsoft.Json.Unity
{
    public class JsonVector3Converter : JsonConverter<Vector3>
    {
        // Source: https://github.com/DeathTBO/Unity3D-Json.Net/blob/master/Assets/Newtonsoft.Json.Unity/JsonVector3Converter.cs
        /// <summary>
        /// This type has an issues with serialization, because normalized/magnitude are properties.
        /// These properties return a new instance of the class, so during serialization.
        /// They cause an endless loop.
        /// This serializes only the x/y/z values skipping those properties.
        /// </summary>
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            JObject j = new() { {"x", value.x}, {"y", value.y}, {"z", value.z}};

            j.WriteTo(writer);
        }

        //CanRead is false which means the default implementation will be used instead.
        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override bool CanWrite => true;

        public override bool CanRead => false;
    }
}