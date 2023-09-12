using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Syncfusion.EJ2.Inputs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VideoGameShowdown.Models.RAWG;

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
        public string? Slug { get; set; }

        [Column(TypeName = "NVARCHAR(255)")]
        public string Name { get; set; }
        public DateTime? Released { get; set; }
        public bool? Tba { get; set; }

        [Column(TypeName = "NVARCHAR(2048)")]
        public string? Background_Image { get; set; }

        public float? Rating { get; set; }

        public int? Rating_Top { get; set; }

        [ForeignKey("RatingId")]
        public ICollection<Rating> Ratings { get; set; }

        public int? Ratings_Count { get; set; } 

        public int? Reviews_Text_Count { get; set; }

        public int? Added { get; set; }
       
        public ICollection<AddedByStatus>? AddedByStatuses { get; set; }

        public int? Metacritic { get; set; }

        public int? Playtime { get; set; }
        
        public int? Suggestions_count { get; set; }

        public DateTime? Updated { get; set; }

        public string? User_Game {  get; set; }

        [Column(TypeName = "NVARCHAR(20)")]
        public string? Saturated_Color { get; set; }

        [Column(TypeName = "NVARCHAR(20)")]
        public string? Dominant_Color { get; set; }

        public ICollection<Platform>? Platforms { get; set; }

        public ICollection<Platform>? Parent_Platforms { get; set; }

        public ICollection<Genre>? Genres { get; set; }

        //public ICollection<Store>? Stores { get; set; }

        //[Column(TypeName = "NVARCHAR(2048)")]
        //public string? Clip { get; set; }

        public ICollection<Tag>? Tags { get; set; }

        [ForeignKey("EsrbRatingId")]
        public string? EsrbRatingId { get; set; }

        [JsonProperty(PropertyName = "Esrb_Rating")]
        public EsrbRating? EsrbRating { get; set; }

        public ICollection<ShortScreenshot>? ShortScreenshots { get; set; }
        #endregion Properties..
    }
}
