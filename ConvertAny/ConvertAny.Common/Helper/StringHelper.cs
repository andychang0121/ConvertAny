using System.Collections.Generic;
using System.Linq;

namespace ConvertAny.Common.Helper
{
    public static class StringHelper
    {
        public static bool GetStringToBoolean(string stringValue)
            => !string.IsNullOrEmpty(stringValue) && stringValue.ToLower() == "true";


        public static bool[] GetStringToArray(string stringValue)
            => stringValue
                .GetStringArray()
                .GetStingArrayToBoolean()
                ?.ToArray();

        public static string[] GetStringArray(this string value)
            => !string.IsNullOrEmpty(value) ? value.Split(',') : null;

        public static IEnumerable<bool> GetStingArrayToBoolean(this string[] values)
            => values?.Select(v => v.ToLower() == "true").ToList();
    }
}
