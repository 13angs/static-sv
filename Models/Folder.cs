using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace static_sv.Models
{
    public class Folder : BaseEntity
    {
        public Folder()
        {
            SubFolders=new List<Folder>();
            Staticfiles=new List<Staticfile>();
        }
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("folder_id")]
        [JsonProperty("folder_id")]
        public virtual long FolderId { get; set; }

        [Column("name")]
        [JsonProperty("name")]
        public virtual string? Name { get; set; }

        [Required]
        [Column("path")]
        [JsonProperty("path")]
        public virtual string? Path { get; set; }

        [Column("parent_folder_id")]
        [JsonProperty("parent_folder_id")]
        public virtual long? ParentFolderId { get; set; }

        [Column("parent_folder")]
        [JsonProperty("parent_folder")]
        public virtual Folder? ParentFolder { get; set; }

        [Column("sub_folders")]
        [JsonProperty("sub_folders")]
        public virtual List<Folder> SubFolders { get; set; }

        [Column("staticfiles")]
        [JsonProperty("staticfiles")]
        public virtual List<Staticfile> Staticfiles { get; set; }
    }
}