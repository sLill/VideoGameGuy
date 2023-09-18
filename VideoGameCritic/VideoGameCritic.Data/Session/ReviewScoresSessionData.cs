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
        }
        #endregion Records..

        #region Properties..
        public List<GameRound> GameRounds { get; set; } = new List<GameRound>();

        public GameRound CurrentRound
            => GameRounds.FirstOrDefault(x => x.UserChoiceId == Guid.Empty);
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
