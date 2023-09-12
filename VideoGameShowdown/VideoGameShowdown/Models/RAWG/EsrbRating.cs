using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class EsrbRating
    {
        #region Properties..
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Id")]
        [Key]
        public string EsrbRatingId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        #endregion Properties..
    }
}
