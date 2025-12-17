using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Common
{
    public static class JsonUtils
    {
        public static string ToJsonAllFields(object obj, bool indented = false)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new AllFieldsContractResolver(),
                Formatting = indented ? Formatting.Indented : Formatting.None
            };

            return JsonConvert.SerializeObject(obj, settings);
        }

        private class AllFieldsContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(
                Type type,
                MemberSerialization memberSerialization)
            {
                var props = new List<JsonProperty>();

                // include all fields: public, private, protected, static, readonly
                var fields = type.GetFields(
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.Static);

                foreach (var f in fields)
                {
                    var jp = base.CreateProperty(f, memberSerialization);
                    jp.Readable = true;
                    jp.Writable = true;
                    props.Add(jp);
                }

                // optional: include properties too if you want
                // comment this block out if you *only* want fields
                var properties = type.GetProperties(
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.Static);

                foreach (var p in properties)
                {
                    var jp = base.CreateProperty(p, memberSerialization);
                    jp.Readable = true;
                    jp.Writable = true;
                    props.Add(jp);
                }

                return props;
            }
        }
    }
}
