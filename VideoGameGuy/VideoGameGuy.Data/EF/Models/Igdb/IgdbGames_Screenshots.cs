namespace VideoGameGuy.Data
{
    public class IgdbGames_Screenshots : ModelBase
    {
        #region Properties..
        public Guid IgdbGames_ScreenshotsId { get; set; }
        public long Games_SourceId { get; set; }
        public long Screenshots_SourceId { get; set; }
        #endregion Properties..

        #region Methods..
        public void Initialize(long games_SourceId, long screenshots_SourceId)
        {
            Games_SourceId = games_SourceId;
            Screenshots_SourceId = screenshots_SourceId;
        }
        #endregion Methods..
    }
}
