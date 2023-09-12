using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VideoGameShowdown.Models.RAWG
{
    public class AddedByStatus
    {
        #region Properties..
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Id")]
        [Key]
        public string AddedByStatusId { get; set; }

        public int? Yet { get; set; }

        public int? Owned { get; set; }

        public int? Beaten { get; set; }

        public int? ToPlay { get; set; }

        public int? Dropped { get; set; }

        public int? Playing { get; set; }

        #endregion Properties..
    }
}
