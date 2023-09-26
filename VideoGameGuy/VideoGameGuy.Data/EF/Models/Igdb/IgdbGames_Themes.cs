namespace VideoGameGuy.Data
{
    public class IgdbGames_Themes : ModelBase
    {
        #region Properties..
        public Guid IgdbGames_ThemesId { get; set; }
        public long Games_SourceId { get; set; }
        public long Themes_SourceId { get; set; }
        #endregion Properties..

        #region Methods..
        public void Initialize(long games_SourceId, long themes_SourceId)
        {
            Games_SourceId = games_SourceId;
            Themes_SourceId = themes_SourceId;
        }
        #endregion Methods..
    }
}
