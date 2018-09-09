using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
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
                ObjectSetter setter = null;
                // if nullable property
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    // get the underlying type property instead of the nullable generic
                    Type underlyingType = new NullableConverter(property.PropertyType).UnderlyingType;
                    setter = (item, value) =>
                    {
                        if (value == null)
                        {
                            property.SetValue(item, null, null);
                            return;
                        }

                        if (value.GetType() == underlyingType)
                        {
                            // no cast needed
                            property.SetValue(item, value);
                        }
                        else
                        {
                            property.SetValue(item, Convert.ChangeType(value, underlyingType));
                        }
                    };
                }
                else
                {
                    setter = (item, value) =>
                    {
                        if (value.GetType() == property.PropertyType)
                        {
                            // no cast needed
                            property.SetValue(item, value);
                        }
                        else
                        {
                            property.SetValue(item, Convert.ChangeType(value, property.PropertyType));
                        }
                    };
                }

                objectSetterCollection.Setters[property.Name] = setter;
            }
            _setterCache[type] = objectSetterCollection;
            return objectSetterCollection;
        }
    }
}
