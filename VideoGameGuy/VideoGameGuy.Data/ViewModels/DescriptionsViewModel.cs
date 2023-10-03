namespace VideoGameGuy.Data
{
    public class DescriptionsViewModel
    {
        #region Properties..
        public Guid SessionId { get; set; }

        public List<DescriptionsRoundViewModel> DescriptionsRounds { get; set; } = new List<DescriptionsRoundViewModel>();

        public DateTime LastUpdateOn { get; set; }
        
        public int HighestScore { get; set; }

        public int CurrentScore
            => DescriptionsRounds.Count(x => x.IsSolved);

        public DescriptionsRoundViewModel CurrentRound
            => DescriptionsRounds.LastOrDefault(x => !x.IsSolved && !x.IsSkipped);
        #endregion Properties..
    }
}
