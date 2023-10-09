using static VideoGameGuy.Data.ScreenshotsSessionItem;

namespace VideoGameGuy.Data
{
    public class ScreenshotsViewModel
    {
        #region Properties..
        public Guid SessionId { get; set; } = Guid.NewGuid();
        public DateTime Igdb_UpdatedOnUtc { get; set; } = DateTime.MinValue;
        public int HighestScore { get; set; }
        public int CurrentScore { get; set; } = 0;
        public ScreenshotsRound CurrentRound { get; set; }
        #endregion Properties..
    }
}
