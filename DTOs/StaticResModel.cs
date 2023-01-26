using Newtonsoft.Json;

namespace static_sv.DTOs
{
    public class StaticResModel
    {
        [JsonProperty("image_url")]
        public string? ImageUrl { get; set; }

        [JsonProperty("signature")]
        public string? Signature { get; set; }

        [JsonProperty("error_code")]
        public string? ErrorCode { get; set; }
    }
}