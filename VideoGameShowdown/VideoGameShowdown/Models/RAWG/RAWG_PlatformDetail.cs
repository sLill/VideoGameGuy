using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class RAWG_PlatformDetail
    {
        #region Properties..
        [Key]
        public int RAWG_PlatformDetailId { get; set; }

        public int id { get; set; }

        [Column(TypeName = "nvarchar(255)")] 
        public string name { get; set; }

        [Column(TypeName = "nvarchar(255)")] 
        public string slug { get; set; }
        public object image { get; set; }
        public object year_end { get; set; }
        public int year_start { get; set; }
        public int games_count { get; set; }

        [Column(TypeName = "nvarchar(2048)")] 
        public string image_background { get; set; }
        #endregion Properties..
    }
}
