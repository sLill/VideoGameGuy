using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class Platform_Game
    {
        #region Properties..
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Platform")]
        public string PlatformId { get; set; }
        public Platform Platform { get; set; }

        [ForeignKey("Game")]
        public string GameId { get; set; }
        public Game Game { get; set; }

        public DateTime? ReleasedAt { get; set; } 
        #endregion Properties..
    }
}
