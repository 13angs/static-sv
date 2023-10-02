
using Newtonsoft.Json;

namespace static_sv.DTOs
{
    // [BindProperties]
    public class StaticQuery
    {
        [JsonProperty("name")]
        public virtual string? Name { get; set; }

        [JsonProperty("limit")]
        public virtual int Limit { get; set; } = 10;

        [JsonProperty("type")]
        public virtual string? Type { get; set; }

        [JsonProperty("Dir")]
        public virtual string? Directory { get; set; }
    }

    public class StaticQueryStore
    {
        public static string All = "all";
    }
}