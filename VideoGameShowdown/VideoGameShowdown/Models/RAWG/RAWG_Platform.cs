using Syncfusion.Licensing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class RAWG_Platform
    {
        #region Properties..
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string Slug { get; set; }

        [Column(TypeName = "nvarchar(2048)")]
        public string Image { get; set; }
        
        public int YearStart { get; set; }
        public int GamesCount { get; set; }

        [Column(TypeName = "nvarchar(2048)")]
        public string ImageBackground { get; set; }
        #endregion Properties..
    }
}


