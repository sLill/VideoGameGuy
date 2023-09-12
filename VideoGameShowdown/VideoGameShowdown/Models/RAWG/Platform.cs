using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class Platform
    {
        #region Properties..
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Id")]
        [Key]
        public string PlatformId { get; set; }

        [Column(TypeName = "NVARCHAR(255)")]
        public string Name { get; set; }

        [Column(TypeName = "NVARCHAR(255)")]
        public string Slug { get; set; }

        [Column(TypeName = "NVARCHAR(2048)")]
        public string Image { get; set; }
        
        public int YearStart { get; set; }
        public int GamesCount { get; set; }

        [Column(TypeName = "NVARCHAR(2048)")]
        public string ImageBackground { get; set; }
        #endregion Properties..
    }
}


