namespace VideoGameCritic.Data
{
    public class GameRoundViewModel
    {
        #region Properties..
        public Game GameOne { get; init; }
        public Game GameTwo { get; init; }
        public Guid UserChoice { get; set; }
        #endregion Properties..
    }
}
