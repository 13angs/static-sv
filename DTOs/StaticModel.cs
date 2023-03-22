using Newtonsoft.Json;

namespace static_sv.DTOs
{
    public class StaticModel
    {
        [JsonProperty("base64_encoded_file")]
        public string? Base64EncodedFile { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("group")]
        public string? Group { get; set; }

        [JsonProperty("add_preview_url")]
        public bool AddPreviewUrl { get; set; }

        [JsonProperty("preview_file")]
        public string? PreviewFile { get; set; }
    }
}