using static VideoGameGuy.Data.ReviewScoresSessionItem;

namespace VideoGameGuy.Data
{
    public class ReviewScoresViewModel
    {
        #region Properties..
        public Guid SessionItemId { get; set; } = Guid.NewGuid();
        public DateTime LastUpdateOn { get; set; }
        public int HighestStreak { get; set; }
        public int Streak { get; set; }

        public string GameOneName { get; set; }
        public string GameTwoName { get; set; }

        public string GameOneImageUri { get; set; }
        public string GameTwoImageUri { get; set; }

        public ReviewScoresRound CurrentRound { get; set; }
        #endregion Properties..
    }
}
