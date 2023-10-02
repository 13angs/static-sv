using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace static_sv.DTOs
{
    public class ContentQueryModel
    {
        [JsonProperty("filetype")]
        [FromQuery(Name = "filetype")]
        public virtual string? Filetype { get; set; }

        [JsonProperty("id")]
        [FromQuery(Name = "id")]
        public virtual long Id { get; set; }

        [JsonProperty("dir")]
        [FromQuery(Name = "dir")]
        public virtual string? Directory { get; set; }
    }
}