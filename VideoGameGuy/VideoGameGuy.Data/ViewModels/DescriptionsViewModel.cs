namespace VideoGameGuy.Data
{
    public class DescriptionsViewModel
    {
        #region Properties..
        public List<DescriptionsRoundViewModel> DescriptionsRounds { get; set; } = new List<DescriptionsRoundViewModel>();

        public DateTime LastUpdateOn { get; set; }
        
        public int HighestStreak { get; set; }

        public int Streak
            => DescriptionsRounds.Count(x => x.IsSolved);

        public DescriptionsRoundViewModel CurrentRound
            => DescriptionsRounds.LastOrDefault(x => !x.IsSolved);
        #endregion Properties..
    }
}
