
using Newtonsoft.Json;

namespace static_sv.DTOs
{
    // [BindProperties]
    public class StaticQuery
    {
        [JsonProperty("name")]
        public virtual string? Name { get; set; }

        [JsonProperty("limit")]
        public virtual int Limit { get; set; }

        [JsonProperty("type")]
        public virtual string? Type { get; set; }
    }

    public class StaticQueryStore
    {
        public static string All = "all";
    }
}