using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class RAWG_Rating
    {
        #region Properties..
        [Key]
        public int RatingId { get; set; }

        public int id { get; set; }

        [Column(TypeName = "nvarchar(255)")] 
        public string title { get; set; }
        public int count { get; set; }
        public double percent { get; set; }
        #endregion Properties..
    }
}

