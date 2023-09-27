namespace VideoGameGuy.Data
{
    public class DescriptionsViewModel
    {
        #region Properties..
        public List<DescriptionsRoundViewModel> DescriptionsRounds { get; set; } = new List<DescriptionsRoundViewModel>();

        public DescriptionsRoundViewModel CurrentRound
            => DescriptionsRounds.LastOrDefault();

        public DateTime LastUpdateOn { get; set; }
        #endregion Properties..
    }
}
