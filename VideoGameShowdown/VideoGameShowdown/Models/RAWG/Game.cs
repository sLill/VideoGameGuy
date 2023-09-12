using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class Game
    {
        #region Properties..
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Id")]
        [Key]
        public string GameId { get; set; }

        [Column(TypeName = "NVARCHAR(255)")]
        public string Slug { get; set; }

        [Column(TypeName = "NVARCHAR(255)")]
        public string Name { get; set; }
        public DateTime? Released { get; set; } 
        public bool Tba { get; set; }

        [Column(TypeName = "NVARCHAR(2048)")]
        public string Background_Image { get; set; }
        public float Rating { get; set; }
        public int Rating_Top { get; set; }
        public int Ratings_Count { get; set; }
        public int Reviews_Text_Count { get; set; }
        public int Added { get; set; }
        public DateTime? Updated { get; set; }


        [ForeignKey("EsrbRating")]
        public string? EsrbRatingId { get; set; }

        [JsonProperty(PropertyName = "Esrb_Rating")]
        public EsrbRating EsrbRating { get; set; }
        #endregion Properties..
    }
}
