using Newtonsoft.Json;

namespace static_sv.DTOs
{
    public class FileModel
    {
        [JsonProperty("name")]
        public virtual string? Name { get; set; }

        [JsonProperty("full_name")]
        public virtual string? FullName { get; set; }

        [JsonProperty("extension")]
        public virtual string? Extension { get; set; }

        [JsonProperty("type")]
        public virtual string? Type { get; set; }
    }
}