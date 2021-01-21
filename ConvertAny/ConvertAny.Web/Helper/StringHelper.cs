using System.Collections.Generic;
using System.Linq;

namespace ConvertAny.Web.Helper
{
    public static class StringHelper
    {
        public static string[] GetStringArray(this string value)
        => !string.IsNullOrEmpty(value) ? value.Split(",") : null;

        public static IEnumerable<bool> GetStingArrayToBoolean(this string[] values)
        => values?.Select(v => v.ToLower() == "true").ToList();
    }
}
