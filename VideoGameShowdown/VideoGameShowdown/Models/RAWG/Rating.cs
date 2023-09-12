using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace VideoGameShowdown.Models
{
    public class Rating
    {
        #region Properties..
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Id")]
        [Key]
        public string RatingId { get; set; }

        [Column(TypeName = "NVARCHAR(255)")]
        public string Title { get; set; }

        public int Count { get; set; }
        public float Percent { get; set; }

        [ForeignKey("Game")]
        public string GameId { get; set; }
        public Game Game { get; set; }
        #endregion Properties..
    }
}

