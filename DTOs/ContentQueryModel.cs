using Newtonsoft.Json;

namespace static_sv.DTOs
{
    public class ContentQueryModel
    {
        [JsonProperty("filetype")]
        public virtual string? Filetype { get; set; }

        [JsonProperty("filetype")]
        public virtual long Id { get; set; }
    }
}