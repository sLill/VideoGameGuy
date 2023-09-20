namespace VideoGameCritic.Data
{
    public class ReviewScoresSessionData : SessionDataBase
    {
        #region Fields..
        #endregion Fields..

        #region Records..
        public record GameRound
        {
            public Guid GameOneId { get; init; }
            public Guid GameTwoId { get; init; }
            public Guid UserChoiceId { get; set; }
            public Guid? WinningGameId { get; set; } = Guid.Empty;
        }
        #endregion Records..

        #region Properties..
        public List<GameRound> GameRounds { get; set; } = new List<GameRound>();

        public int HighestStreak { get; set; }

        public GameRound CurrentRound
            => GameRounds.FirstOrDefault(x => x.UserChoiceId == Guid.Empty);
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
