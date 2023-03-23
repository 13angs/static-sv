using Newtonsoft.Json;
using static_sv.Models;

namespace static_sv.DTOs
{
    public class StaticfileModel : Staticfile
    {
        [JsonProperty("url")]
        public virtual string? Url { get; set; }
    }
}