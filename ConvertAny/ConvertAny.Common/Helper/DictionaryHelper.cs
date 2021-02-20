using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace ConvertAny.Common.Helper
{
    public static class DictionaryHelper
    {
        public static Dictionary<string, string> GetDictionaryValue(this NameValueCollection nameValue)
            => nameValue.Count != 0
                ? nameValue
                    .AllKeys
                    .ToDictionary(key => key, key => nameValue[key])
                : new Dictionary<string, string>();

        public static string GetDictionaryValue(this Dictionary<string, string> dict, string key)
        {
            if (dict.Count == 0) return string.Empty;

            bool isExist = dict.TryGetValue(key, out _);

            return isExist ? dict[key] : string.Empty;
        }
    }
}
