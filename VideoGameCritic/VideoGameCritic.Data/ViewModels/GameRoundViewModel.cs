namespace VideoGameCritic.Data
{
    public class GameRoundViewModel
    {
        #region Properties..
        public Game GameOne { get; init; }
        public Game GameTwo { get; init; }
        public Guid UserChoice { get; set; }
        public Guid? WinningGameId { get; set; } = Guid.Empty;
        #endregion Properties..
    }
}
