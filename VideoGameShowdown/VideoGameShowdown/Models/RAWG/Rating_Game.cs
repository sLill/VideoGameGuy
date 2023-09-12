using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VideoGameShowdown.Models.RAWG
{
    public class Rating_Game
    {
        #region Properties..
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("RatingdId")]
        public Rating Rating { get; set; }

        [ForeignKey("GameId")]
        public Game Game { get; set; }
        #endregion Properties..
    }
}
