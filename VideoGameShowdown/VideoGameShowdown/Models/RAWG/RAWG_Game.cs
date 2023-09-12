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
        public double Rating { get; set; }
        public int RatingTop { get; set; }
        public int RatingsCount { get; set; }
        public int ReviewsTextCount { get; set; }
        public int Added { get; set; }
        public DateTime Updated { get; set; }

        // Relationships
        [ForeignKey("EsrbRating")]
        public int EsrbRatingId { get; set; }
        public RAWG_EsrbRating EsrbRating { get; set; }

        public List<Rating> Ratings { get; set; }
        public List<PlatformGame> Platforms { get; set; }
        public List<GenreGame> Genres { get; set; }
        public List<StoreGame> Stores { get; set; }
        public List<TagGame> Tags { get; set; }
        public List<RAWG_ShortScreenshot> ShortScreenshots { get; set; }
        #endregion Properties..
    }
}
