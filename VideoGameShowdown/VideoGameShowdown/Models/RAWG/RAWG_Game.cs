using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class RAWG_Game
    {
        #region Properties..
        [Key]
        public int GameId { get; set; }

        public int id { get; set; }

        [Column(TypeName = "nvarchar(255)")] 
        public string slug { get; set; }

        [Column(TypeName = "nvarchar(255)")] 
        public string name { get; set; }
        public DateTime released { get; set; }
        public bool tba { get; set; }

        [Column(TypeName = "nvarchar(2048)")] 
        public string background_image { get; set; }

        public double rating { get; set; }
        public int rating_top { get; set; }
        public List<RAWG_Rating> ratings { get; set; }
        public int ratings_count { get; set; }
        public int reviews_text_count { get; set; }
        public int added { get; set; }
        public RAWG_AddedByStatus added_by_status { get; set; }
        public int metacritic { get; set; }
        public int playtime { get; set; }
        public int suggestions_count { get; set; }
        public DateTime updated { get; set; }
        public object user_game { get; set; }
        public int reviews_count { get; set; }

        [Column(TypeName = "nvarchar(10)")] 
        public string saturated_color { get; set; }

        [Column(TypeName = "nvarchar(10)")] 
        public string dominant_color { get; set; }
        public List<RAWG_Platform> platforms { get; set; }
        public List<RAWG_ParentPlatform> parent_platforms { get; set; }
        public List<Genre> genres { get; set; }
        public List<RAWG_Store> stores { get; set; }
        public object clip { get; set; }
        public List<RAWG_Tag> tags { get; set; }
        public RAWG_EsrbRating esrb_rating { get; set; }
        public List<RAWG_ShortScreenshot> short_screenshots { get; set; }
        #endregion Properties..
    }
}
