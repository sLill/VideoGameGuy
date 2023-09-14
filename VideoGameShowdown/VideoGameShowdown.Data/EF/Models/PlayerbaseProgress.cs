namespace VideoGameShowdown.Data
{
    public class PlayerbaseProgress
    {
        #region Properties..
        public Guid PlayerbaseProgressId { get; set; }

        public Guid GameId { get; set; }
        public Game Game { get; set; }

        public int? OwnTheGame {  get; set; }

        public int? BeatTheGame {  get; set; }

        public double? BeatTheGame_Percent { get; set; }
        #endregion Properties..
    }
}
