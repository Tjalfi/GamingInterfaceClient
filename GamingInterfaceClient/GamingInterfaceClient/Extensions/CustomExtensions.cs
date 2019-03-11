using System.Collections.Generic;

namespace GamingInterfaceClient.Extensions
{
    public static class CustomExtensions
    {
        public static void AddOrSet(this IDictionary<string, object> dict, string key, object value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static void AddOrSet(this IDictionary<int, string> dict, int key, string value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }
    }
}
