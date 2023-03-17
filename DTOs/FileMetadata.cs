using Newtonsoft.Json;

namespace static_sv.DTOs
{
    public class FileMetadata
    {
        public FileMetadata()
        {
            CreatedDate=DateTime.Now;
        }
        [JsonProperty("file_name")]
        public virtual string? FileName { get; set; }
        
        [JsonProperty("file_size")]
        public virtual string? FileSize { get; set; }

        [JsonProperty("mime_type")]
        public virtual string? MimeType { get; set; }

        [JsonProperty("created_date")]
        public virtual DateTime CreatedDate { get; set; }

        [JsonProperty("modified_date")]
        public virtual DateTime ModifiedDate { get; set; }

        [JsonProperty("author")]
        public virtual string? Author { get; set; }

        [JsonProperty("description")]
        public virtual string? Description { get; set; }
    }
}