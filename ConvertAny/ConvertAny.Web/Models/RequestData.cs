using Newtonsoft.Json;

namespace ConvertAny.Web.Models
{
    public struct RequestData
    {
        [JsonProperty("base64")]
        public string Base64 { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        public bool IsCustomSize => !string.IsNullOrEmpty(CustomWidth) && !string.IsNullOrEmpty(CustomHeight);

        [JsonProperty("customWidth")]
        public string CustomWidth { get; set; }

        [JsonProperty("customHeight")]
        public string CustomHeight { get; set; }

    }
}
