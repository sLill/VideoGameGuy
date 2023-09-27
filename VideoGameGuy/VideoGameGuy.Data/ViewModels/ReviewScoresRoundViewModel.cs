namespace VideoGameGuy.Data
{
    public class ReviewScoresRoundViewModel
    {
        #region Properties..
        public RawgGame GameOne { get; init; }
        public RawgGame GameTwo { get; init; }
        public Guid UserChoice { get; set; }
        public Guid? WinningGameId { get; set; } = Guid.Empty;
        #endregion Properties..
    }
}
