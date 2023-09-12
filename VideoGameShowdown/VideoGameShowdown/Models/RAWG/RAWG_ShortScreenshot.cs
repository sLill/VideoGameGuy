using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VideoGameShowdown.Models
{
    public class RAWG_ShortScreenshot
    {
        #region Properties..
        [Key]
        public int Id { get; set; }
        public string Image { get; set; }

        [ForeignKey("Game")]
        public int GameId { get; set; }
        public RAWG_Game Game { get; set; }
        #endregion Properties..
    }
}
