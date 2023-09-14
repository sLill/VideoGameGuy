using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Data
{
    public class PlayerbaseProgress
    {
        #region Properties..
        public Guid PlayerbaseProgressId { get; set; }

        public Guid GameId { get; set; }
        public Game Game { get; set; }

        public double? OwnTheGame {  get; set; }

        public double? BeatTheGame {  get; set; }

        [NotMapped]
        public double? BeatTheGame_Percent { get; set; }
        #endregion Properties..
    }
}
