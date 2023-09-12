using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace VideoGameShowdown.Models
{
    public class Tag
    {
        #region Properties..
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Id")]
        [Key]
        public string TagId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int GamesCount { get; set; }
        public string ImageBackground { get; set; }
        #endregion Properties..
    }
}

