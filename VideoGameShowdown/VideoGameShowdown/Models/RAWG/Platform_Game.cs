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

        [ForeignKey("PlatformId")]
        public Platform Platform { get; set; }

        [ForeignKey("GameId")]
        public Game Game { get; set; }
        #endregion Properties..
    }
}
