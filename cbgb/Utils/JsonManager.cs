using cbgb.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace cbgb.Utils
{
    class JsonManager
    {
        private static JsonSerializer serializer = new JsonSerializer();

        public static T Load<T>(string path)
        {
            try
            {
                using (var sr = new StreamReader(path))
                using (var reader = new JsonTextReader(sr))
                {
                    var obj = serializer.Deserialize<T>(reader);
                    return (T)Convert.ChangeType(obj, typeof(T));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
            }
            return default(T);
        }

        public static void Save<T>(string path,T obj)
        {
            using (var sw = new StreamWriter(path))
            using (var writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, obj);
            }
        }
    }
}
