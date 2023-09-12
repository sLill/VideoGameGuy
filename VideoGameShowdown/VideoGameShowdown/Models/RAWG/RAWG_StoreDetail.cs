using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class RAWG_StoreDetail
    {
        #region Properties..
        [Key]
        public int StoreDetailId { get; set; }

        public int id { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string name { get; set; }

        [Column(TypeName = "nvarchar(255)")] 
        public string slug { get; set; }

        [Column(TypeName = "nvarchar(2048)")]
        public string domain { get; set; }
        public int games_count { get; set; }

        [Column(TypeName = "nvarchar(2048)")]
        public string image_background { get; set; } 
        #endregion Properties..
    }
}
