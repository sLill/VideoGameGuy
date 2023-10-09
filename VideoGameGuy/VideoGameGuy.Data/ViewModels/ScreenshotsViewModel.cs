using static VideoGameGuy.Data.ScreenshotsSessionItem;

namespace VideoGameGuy.Data
{
    public class ScreenshotsViewModel
    {
        #region Properties..
        public Guid SessionId { get; set; } = Guid.NewGuid();

        public List<ScreenshotsRound> ScreenshotsRounds { get; set; } = new List<ScreenshotsRound>();
        public ScreenshotsRound SelectedRound { get; set; }

        public DateTime Igdb_UpdatedOnUtc { get; set; } = DateTime.MinValue;
        public int HighestScore { get; set; }
        public int CurrentScore { get; set; } = 0;
        #endregion Properties..
    }
}
