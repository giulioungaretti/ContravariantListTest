using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    public class JsonItemConverter : Newtonsoft.Json.Converters.CustomCreationConverter<Item>
    {
        public override Item Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        public Item Create(Type objectType, JObject jObject)
        {
            string? type = (string?)jObject.Property("type");
            return type switch
            {
                "int" => new IntItem(),
                "string" => new StringItem(),
                _ => throw new JsonException(String.Format("Type: {0} is not supported!", type)),
            };
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            var target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }
    }

    public abstract class Item
    {
        [JsonProperty("type", Required = Required.Always)]
        public string ValueType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        internal Item() { }

    }

    public class StringItem : Item
    {
        [JsonProperty("value")]
        public string? Value { get; set; }
    
        [JsonProperty("type")]
        public new string ValueType = "int";

        internal StringItem() { }
    }

    public class IntItem : Item
    {
        [JsonProperty("value")]
        public int? Value { get; set; }

        [JsonProperty("type")]
        public new string ValueType = "string";
        internal IntItem() { }


    }

    class Program
    {
        static void Main(string[] args)
        {
            // json string
            var json = "[{\"value\":5,\"type\":\"int\",\"name\":\"bar\"},{\"value\":\"some thing\",\"type\":\"string\",\"name\":\"foo \"},{\"value\":2,\"type\":\"int\",\"name\":\"baz\"}]";

            // The above is deserialized into a list of Items, instead of a hetrogenous list of
            // IntItem and StringItem
            var result = JsonConvert.DeserializeObject<List<Item>>(json, new JsonItemConverter());
            if (result != null) {
                foreach (var r in result)
                {
                    switch (r)
                    {
                        case IntItem i:
                            Console.WriteLine($"{i.Name}={i.Value} of type {i.ValueType}");
                            break;
                        case StringItem s:
                            Console.WriteLine($"{s.Name}:{s.Value} of type {s.ValueType}");
                            break;
                        default:
                            throw new ArgumentException(
                                message: "Unexpected type",
                                paramName: nameof(r));

                    }
                }
            };
     

            var formattedJson = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine($"as json {formattedJson}");

            var testData = new List<Item>();
            var sitem = new StringItem();
            testData.Add(sitem);
            var iitem = new IntItem();
            testData.Add(iitem);
            var formattedJson2 = JsonConvert.SerializeObject(testData, Formatting.Indented);
            Console.WriteLine($"as json {formattedJson2}");

        }
    }
}