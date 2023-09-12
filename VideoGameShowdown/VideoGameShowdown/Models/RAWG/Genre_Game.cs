using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class Genre_Game
    {
        #region Properties..
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Genre")]
        public string GenreId { get; set; }
        public Genre Genre { get; set; }

        [ForeignKey("Game")]
        public string GameId { get; set; }
        public Game Game { get; set; }
        #endregion Properties..
    }
}