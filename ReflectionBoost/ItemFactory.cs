using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace ReflectionBoost
{
    public class ItemFactory
    {
        private IDictionary<Type, ObjectSetterCollection> _setterCache;

        public ItemFactory()
        {
            _setterCache = new ConcurrentDictionary<Type, ObjectSetterCollection>();
        }

        public T Create<T>(IDictionary<string, object> properties) where T : new()
        {
            var t = new T();
            Type type = t.GetType();

            ObjectSetterCollection objectSetterCollection;

            if (!_setterCache.TryGetValue(type, out objectSetterCollection))
            {
                // add to cache
                objectSetterCollection = AddToCache(type);    
            }

            foreach (var property in properties)
            {
                var setter = objectSetterCollection.Setters[property.Key];
                setter.Invoke(t, property.Value);
            }

            return t;
        }

        private ObjectSetterCollection AddToCache(Type type)
        {
            ObjectSetterCollection objectSetterCollection = new ObjectSetterCollection();
            PropertyInfo[] propertyInfo = type.GetProperties();

            foreach (PropertyInfo property in propertyInfo)
            {
                ObjectSetter setter = type.GetProperty(property.Name).SetValue;
                objectSetterCollection.Setters[property.Name] = setter;
            }
            _setterCache[type] = objectSetterCollection;
            return objectSetterCollection;
        }
    }
}
