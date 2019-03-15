using Newtonsoft.Json;
using System.Collections.Generic;

namespace KBEngine
{
    public static class CBJsonUntity
    {
        /// <summary>
        /// 将字典类型序列化为json字符串
        /// </summary>
        /// <typeparam name="TKey">字典key</typeparam>
        /// <typeparam name="TValue">字典value</typeparam>
        /// <param name="dict">要序列化的字典数据</param>
        /// <returns>json字符串</returns>
        public static string SerializeDictionaryToJsonString<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            if (dict.Count == 0)
                return "";

            string jsonStr = JsonConvert.SerializeObject(dict);
            return jsonStr;
        }
        public static string SerializeListToJsonString<T>(List<T> list)
        {
            if (list.Count == 0)
                return "";

            string jsonStr = JsonConvert.SerializeObject(list);
            return jsonStr;
        }

        public static string SerializeToJsonString(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 将json字符串反序列化为字典类型
        /// </summary>
        /// <typeparam name="TKey">字典key</typeparam>
        /// <typeparam name="TValue">字典value</typeparam>
        /// <param name="jsonStr">json字符串</param>
        /// <returns>字典数据</returns>
        public static Dictionary<TKey, TValue> DeserializeStringToDictionary<TKey, TValue>(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
                return new Dictionary<TKey, TValue>();

            Dictionary<TKey, TValue> jsonDict = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(jsonStr);

            return jsonDict;

        }

        public static List<T> DeserializeStringToList<T>(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
                return new List<T>();

            List<T> jsonlist = JsonConvert.DeserializeObject<List<T>>(jsonStr);

            return jsonlist;

        }

        public static object DeserializeObject(string value)
        {
            return JsonConvert.DeserializeObject(value);
        }
    }
}