using Newtonsoft.Json;
using static_sv.Models;

namespace static_sv.DTOs
{
    public class StaticDirectoryModel
    {
        public StaticDirectoryModel()
        {
            Folders=new List<Folder>();
            Staticfiles=new List<StaticfileModel>();
        }

        [JsonProperty("folders")]
        public virtual List<Folder> Folders { get; set; }

        [JsonProperty("staticfiles")]
        public virtual List<StaticfileModel> Staticfiles { get; set; }
    }
}