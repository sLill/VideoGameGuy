using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class Store
    {
        #region Properties..
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Id")]
        [Key]
        public string StoreId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Domain { get; set; }
        public int GamesCount { get; set; }
        public string ImageBackground { get; set; }
        #endregion Properties..
    }
}
