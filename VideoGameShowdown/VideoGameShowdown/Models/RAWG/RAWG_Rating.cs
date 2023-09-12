using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class RAWG_Rating
    {
        #region Properties..
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string Title { get; set; }

        public int Count { get; set; }
        public float Percent { get; set; }

        [ForeignKey("Game")]
        public int GameId { get; set; }
        public RAWG_Game RAWG_Game { get; set; }
        #endregion Properties..
    }
}

