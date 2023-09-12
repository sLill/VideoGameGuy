using Syncfusion.EJ2.Inputs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace VideoGameShowdown.Models
{
    public class RAWG_Game
    {
        #region Properties..
        [Key]
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public DateTime Released { get; set; }
        public bool Tba { get; set; }
        public string BackgroundImage { get; set; }
        public float Rating { get; set; }
        public int RatingTop { get; set; }
        public int RatingsCount { get; set; }
        public int ReviewsTextCount { get; set; }
        public int Added { get; set; }
        public DateTime Updated { get; set; }

        // Relationships
        [ForeignKey("EsrbRating")]
        public int EsrbRatingId { get; set; }
        public RAWG_EsrbRating EsrbRating { get; set; }
        #endregion Properties..
    }
}
