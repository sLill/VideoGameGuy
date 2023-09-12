using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class RAWG_Rating
    {
        #region Properties..
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Count { get; set; }
        public double Percent { get; set; }

        [ForeignKey("Game")]
        public int GameId { get; set; }
        public RAWG_Game Game { get; set; }
        #endregion Properties..
    }
}

