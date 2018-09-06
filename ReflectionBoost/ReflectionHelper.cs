using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReflectionBoost
{
    public static class ReflectionHelper
    {
        public static T DictionaryToObject<T>(IDictionary<string, object> dict) where T : new()
        {
            var t = new T();
            Type type = t.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!dict.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                    continue;

                KeyValuePair<string, object> item = dict.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));
                type.GetProperty(property.Name).SetValue(t, item.Value, null);
            }
            return t;
        }

        public static T DictionaryToObjectCached<T>(ItemFactory itemFactory, IDictionary<string, object> dict) where T : new()
        {
            return itemFactory.Create<T>(dict);
        }
    }
}
