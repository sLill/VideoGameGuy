namespace VideoGameGuy.Data
{
    public class ScreenshotsSessionItem
    {
        #region Records..
        public record ImageRecord
        {
            public string Value { get; set; }
        }

        public record ScreenshotsRound
        {
            public string GameTitle { get; set; }
            public List<ImageRecord> ImageCollection { get; set; }
            public bool IsSolved { get; set; }
        }
        #endregion Records..

        #region Properties..
        public List<ScreenshotsRound> ScreenshotsRounds { get; set; }

        public int SelectedRoundIndex { get; set; }

        public int HighestScore { get; set; }

        public int CurrentScore
            => ScreenshotsRounds?.Count(x => x.IsSolved) ?? 0;
        #endregion Properties..
    }
}
