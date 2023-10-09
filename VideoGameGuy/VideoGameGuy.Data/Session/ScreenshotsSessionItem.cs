namespace VideoGameGuy.Data
{
    public class ScreenshotsSessionItem
    {
        #region Records..
        public record ScreenshotsRound
        {
            public string GameTitle { get; set; }
            public bool IsSolved { get; set; }
        }
        #endregion Records..

        #region Properties..
        public List<ScreenshotsRound> ScreenshotsRounds { get; set; } = new List<ScreenshotsRound>();

        public int HighestScore { get; set; }

        public int CurrentScore
            => ScreenshotsRounds.Count(x => x.IsSolved);

        public ScreenshotsRound CurrentRound
            => ScreenshotsRounds.LastOrDefault(x => !x.IsSolved);
        #endregion Properties..
    }
}
