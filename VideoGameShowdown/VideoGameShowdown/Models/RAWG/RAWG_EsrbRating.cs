using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class RAWG_EsrbRating
    {
        #region Properties..
        [Key]
        public int EsrbRatingId { get; set; }

        public int id { get; set; }

        [Column(TypeName = "nvarchar(255)")] 
        public string name { get; set; }

        [Column(TypeName = "nvarchar(255)")] 
        public string slug { get; set; } 
        #endregion Properties..
    }
}
