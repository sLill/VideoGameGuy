using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VideoGameShowdown.Models
{
    public class RAWG_ShortScreenshot
    {
        #region Properties..
        [Key]
        public int ShortScreenshotId { get; set; }

        public int id { get; set; }

        [Column(TypeName = "nvarchar(2048)")]
        public string image { get; set; }
        #endregion Properties..
    }
}
