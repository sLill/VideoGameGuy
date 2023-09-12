using Syncfusion.Licensing;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VideoGameShowdown.Models
{
    public class PlatformGame
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Platform")]
        public int PlatformId { get; set; }
        public Platform Platform { get; set; }

        [ForeignKey("Game")]
        public int GameId { get; set; }
        public RAWG_Game Game { get; set; }

        public DateTime ReleasedAt { get; set; }
    }

    public class GenreGame
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Genre")]
        public int GenreId { get; set; }
        public RAWG_Genre Genre { get; set; }

        [ForeignKey("Game")]
        public int GameId { get; set; }
        public RAWG_Game Game { get; set; }
    }

    public class StoreGame
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Store")]
        public int StoreId { get; set; }
        public RAWG_Store Store { get; set; }

        [ForeignKey("Game")]
        public int GameId { get; set; }
        public RAWG_Game Game { get; set; }
    }

    public class TagGame
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Tag")]
        public int TagId { get; set; }
        public RAWG_Tag Tag { get; set; }

        [ForeignKey("Game")]
        public int GameId { get; set; }
        public RAWG_Game Game { get; set; }
    }

}
