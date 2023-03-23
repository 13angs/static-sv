using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace static_sv.Models
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            CreatedDate=DateTime.Now;
            ModifiedDate=DateTime.Now;
        }
        [Column("created_date")]
        [JsonProperty("created_date")]
        public virtual DateTime CreatedDate { get; set; }

        [Column("modified_date")]
        [JsonProperty("modified_date")]
        public virtual DateTime ModifiedDate { get; set; }
        
    }
}