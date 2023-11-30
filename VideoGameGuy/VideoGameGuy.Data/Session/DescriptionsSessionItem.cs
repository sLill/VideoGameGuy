namespace VideoGameGuy.Data
{
    public class DescriptionsSessionItem : SessionItemBase
    {
        #region Records..
        public record DescriptionsRound
        {
            public string GameTitle { get; set; }
            public string GameSlug { get; set; }
            public string GameDescription { get; set; }
            public bool IsSolved { get; set; }
            public bool IsSkipped { get; set; }
        }
        #endregion Records..

        #region Properties..
        public List<DescriptionsRound> DescriptionsRounds { get; set; } = new List<DescriptionsRound>();

        public int HighestScore { get; set; }

        public int CurrentScore
            => DescriptionsRounds.Count(x => x.IsSolved);

        public DescriptionsRound CurrentRound
            => DescriptionsRounds.LastOrDefault(x => !x.IsSolved && !x.IsSkipped);
        #endregion Properties..
    }
}
