using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace static_sv.Models
{
    public class Staticfile : BaseEntity
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("staticfile_id")]
        [JsonProperty("staticfile_id")]
        public virtual long StaticfileId { get; set; }

        [Column("name")]
        [JsonProperty("name")]
        public virtual string? Name { get; set; }

        [Required]
        [Column("path")]
        [JsonProperty("path")]
        public virtual string? Path { get; set; }

        [Required]
        [Column("type")]
        [JsonProperty("type")]
        public virtual string? Type { get; set; }

        [Required]
        [Column("size")]
        [JsonProperty("size")]
        public virtual long Size { get; set; }

        [Column("folder_id")]
        [JsonProperty("folder_id")]
        public virtual long? FolderId { get; set; }

        [Column("folder")]
        [JsonProperty("folder")]
        public virtual Folder? Folder { get; set; }

        [Column("timestamp")]
        [JsonProperty("timestamp")]
        public virtual long Timestamp { get; set; }

        [Column("file_data")]
        [JsonProperty("file_data")]
        public virtual byte[]? FileData { get; set; }

        [Column("parent_file_id")]
        [JsonProperty("parent_file_id")]
        public virtual long? ParentFileId { get; set; }

        [Column("parent_file")]
        [JsonProperty("parent_file")]
        public virtual Staticfile? ParentFile { get; set; }

        [Column("related_files")]
        [JsonProperty("related_files")]
        public virtual List<Staticfile>? RelatedFiles { get; set; }
    }
}