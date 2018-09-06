
using System.Collections.Generic;

namespace ReflectionBoost
{
    public class ObjectSetterCollection
    {
        public Dictionary<string, ObjectSetter> Setters { get; set; }

        public ObjectSetterCollection()
        {
            Setters = new Dictionary<string, ObjectSetter>();
        }
    }
}
