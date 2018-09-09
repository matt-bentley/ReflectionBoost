using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ReflectionBoost
{
    class Program
    {
        static ItemFactory itemFactory = new ItemFactory();

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("Id", 3);
            properties.Add("Name", "3");
            properties.Add("CreatedDate", DateTime.Now);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 1000000; i++)
            {
                GetNewObject(properties);
            }        

            sw.Stop();

            Console.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms, press any key to exit...");
            Console.ReadKey();
        }

        static void GetNewObject(Dictionary<string, object> properties)
        {
            //Reflector r = ReflectionHelper.DictionaryToObject<Reflector>(properties);
            Reflector r = ReflectionHelper.DictionaryToObjectCached<Reflector>(itemFactory, properties);
            //var json = JsonConvert.SerializeObject(properties);
            //var r = JsonConvert.DeserializeObject<Reflector>(json);
            //Reflector r = new Reflector()
            //{
            //    Id = (int)properties["Id"],
            //    Name = properties["Name"].ToString(),
            //    CreatedDate = (DateTime)properties["CreatedDate"]
            //};
        }
    }
}
