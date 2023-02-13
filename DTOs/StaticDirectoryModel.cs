using Newtonsoft.Json;

namespace static_sv.DTOs
{
    public class StaticDirectoryModel
    {
        public StaticDirectoryModel()
        {
            Directories=new List<DirectoryModel>();
            Files=new List<FileModel>();
        }

        [JsonProperty("directories")]
        public virtual List<DirectoryModel> Directories { get; set; }

        [JsonProperty("files")]
        public virtual List<FileModel> Files { get; set; }
    }
}