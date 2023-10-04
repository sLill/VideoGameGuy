using VideoGameGuy.Data;

namespace VideoGameGuy.Data
{
    public class ReviewScoresSessionItem : SessionItemBase
    {
        #region Records..
        public record ReviewScoresRound
        {
            public Guid GameOneId { get; init; }
            public Guid GameTwoId { get; init; }
            public Guid UserChoiceId { get; set; }
            public Guid? WinningGameId { get; set; } = Guid.Empty;
        }
        #endregion Records..

        #region Properties..
        public List<ReviewScoresRound> ReviewScoresRounds { get; set; } = new List<ReviewScoresRound>();

        public int HighestStreak { get; set; }

        public ReviewScoresRound CurrentRound
            => ReviewScoresRounds.FirstOrDefault(x => x.UserChoiceId == Guid.Empty);
        #endregion Properties..
    }
}
