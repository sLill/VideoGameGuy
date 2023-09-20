namespace VideoGameCritic.Data
{
    public class ReviewScoresViewModel
    {
        #region Properties..
        public List<GameRoundViewModel> GameRounds { get; set; } = new List<GameRoundViewModel>();

        public DateTime LastUpdateOn { get; set; }

        public int HighestStreak { get; set; }

        public int Streak
            => GameRounds.Count(x => x.UserChoice != default && x.UserChoice == x.WinningGameId);

        public GameRoundViewModel CurrentRound
            => GameRounds.LastOrDefault();
        #endregion Properties..
    }
}
