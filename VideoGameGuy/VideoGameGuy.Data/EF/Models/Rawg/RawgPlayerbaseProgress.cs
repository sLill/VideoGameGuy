using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameGuy.Data
{
    public class RawgPlayerbaseProgress : ModelBase
    {
        #region Properties..
        public Guid RawgPlayerbaseProgressId { get; set; }

        public Guid GameId { get; set; }
        public RawgGame Game { get; set; }

        public double? OwnTheGame {  get; set; }

        public double? BeatTheGame {  get; set; }

        [NotMapped]
        public double? BeatTheGame_Percent { get; set; }
        #endregion Properties..
    }
}
