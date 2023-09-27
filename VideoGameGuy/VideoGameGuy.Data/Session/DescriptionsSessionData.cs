namespace VideoGameGuy.Data
{
    public class DescriptionsSessionData : SessionDataBase
    {
        #region Records..
        public record DescriptionsRound
        {
            public string GameTitle { get; set; }
            public string GameMediaUrl { get; set; }
            public string GameDescription { get; set; }
            public bool IsCorrect { get; set; }
        }
        #endregion Records..

        #region Properties..
        public List<DescriptionsRound> DescriptionsRounds { get; set; } = new List<DescriptionsRound>();

        public DescriptionsRound CurrentRound
            => DescriptionsRounds.LastOrDefault();
        #endregion Properties..
    }
}
