using Newtonsoft.Json;

namespace ConvertAny.Web.Helper
{
    public static class JsonHelper
    {
        public static string Serialize<T>(this T data) => JsonConvert.SerializeObject(data);

        public static T Deserialize<T>(this string stringValue) => JsonConvert.DeserializeObject<T>(stringValue);
    }
}
