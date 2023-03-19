
using Newtonsoft.Json;

namespace static_sv.DTOs
{
    // [BindProperties]
    public class StaticQuery
    {
        [JsonProperty("is")]
        public virtual string? Is { get; set; }

        [JsonProperty("group_id")]
        public virtual string? GroupId { get; set; }

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