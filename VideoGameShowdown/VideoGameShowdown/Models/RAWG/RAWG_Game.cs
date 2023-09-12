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
        public string Background_Image { get; set; }
        public float Rating { get; set; }
        public int Rating_Top { get; set; }
        public int Ratings_Count { get; set; }
        public int Reviews_Text_Count { get; set; }
        public int Added { get; set; }
        public DateTime Updated { get; set; }

        [ForeignKey("Esrb_Rating")]
        public int EsrbRatingId { get; set; }
        public RAWG_EsrbRating Esrb_Rating { get; set; }
        #endregion Properties..
    }
}
