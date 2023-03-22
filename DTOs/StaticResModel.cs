using Newtonsoft.Json;

namespace static_sv.DTOs
{
    public class StaticResModel
    {
        [JsonProperty("file_url")]
        public string? FileUrl { get; set; }

        [JsonProperty("signature")]
        public string? Signature { get; set; }

        [JsonProperty("error_code")]
        public string? ErrorCode { get; set; }

        [JsonProperty("preview_url")]
        public string? PreviewUrl { get; set; }
    }
}