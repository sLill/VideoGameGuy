namespace VideoGameGuy.Data
{
    public class DescriptionsSessionData : SessionDataBase
    {
        #region Records..
        public record DescriptionsRound
        {
            public string GameTitle { get; set; }
            public string GameDescription { get; set; }
            public bool IsSolved { get; set; }
            public bool IsSkipped { get; set; }
        }
        #endregion Records..

        #region Properties..
        public Guid SessionId { get; set; } = Guid.NewGuid();

        public List<DescriptionsRound> DescriptionsRounds { get; set; } = new List<DescriptionsRound>();

        public int HighestScore { get; set; }

        public int CurrentScore
            => DescriptionsRounds.Count(x => x.IsSolved);

        public DescriptionsRound CurrentRound
            => DescriptionsRounds.LastOrDefault(x => !x.IsSolved && !x.IsSkipped);
        #endregion Properties..
    }
}
