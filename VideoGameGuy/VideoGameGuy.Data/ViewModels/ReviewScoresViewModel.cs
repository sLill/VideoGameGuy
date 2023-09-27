namespace VideoGameGuy.Data
{
    public class ReviewScoresViewModel
    {
        #region Properties..
        public List<ReviewScoresRoundViewModel> ReviewScoresRounds { get; set; } = new List<ReviewScoresRoundViewModel>();

        public DateTime LastUpdateOn { get; set; }

        public int HighestStreak { get; set; }

        public int Streak
            => ReviewScoresRounds.Count(x => x.UserChoice != default && x.UserChoice == x.WinningGameId);

        public ReviewScoresRoundViewModel CurrentRound
            => ReviewScoresRounds.LastOrDefault();
        #endregion Properties..
    }
}
