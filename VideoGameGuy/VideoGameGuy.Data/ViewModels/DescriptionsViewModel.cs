using static VideoGameGuy.Data.DescriptionsSessionItem;

namespace VideoGameGuy.Data
{
    public class DescriptionsViewModel
    {
        #region Properties..
        public Guid SessionId { get; set; } = Guid.NewGuid();

        public DateTime Igdb_UpdatedOnUtc { get; set; } = DateTime.MinValue;

        public TimeSpan TimeRemaining { get; set; }

        public int HighestScore { get; set; }

        public int CurrentScore { get; set; } = 0;

        public DescriptionsRound CurrentRound { get; set; }
        #endregion Properties..
    }
}
