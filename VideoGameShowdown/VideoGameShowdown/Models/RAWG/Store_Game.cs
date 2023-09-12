using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class Store_Game
    {
        #region Properties..
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Store")]
        public string StoreId { get; set; }
        public Store Store { get; set; }

        [ForeignKey("Game")]
        public string GameId { get; set; }
        public Game Game { get; set; }
        #endregion Properties..
    }
}
