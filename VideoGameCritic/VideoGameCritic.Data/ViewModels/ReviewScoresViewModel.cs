namespace VideoGameCritic.Data
{
    public class ReviewScoresViewModel
    {
        #region Properties..
        public List<GameRoundViewModel> GameRounds {  get; set; } = new List<GameRoundViewModel>();

        public GameRoundViewModel CurrentRound
            => GameRounds.LastOrDefault();
        #endregion Properties..
    }
}
