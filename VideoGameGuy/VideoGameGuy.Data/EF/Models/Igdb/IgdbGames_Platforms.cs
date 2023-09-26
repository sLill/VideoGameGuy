namespace VideoGameGuy.Data
{
    public class IgdbGames_Platforms : ModelBase
    {
        #region Properties..
        public Guid IgdbGames_PlatformsId { get; set; }
        public long Games_SourceId { get; set; }
        public long Platforms_SourceId { get; set; }
        #endregion Properties..

        #region Methods..
        public void Initialize(long games_SourceId, long platforms_SourceId)
        {
            Games_SourceId = games_SourceId;
            Platforms_SourceId = platforms_SourceId;
        }
        #endregion Methods..
    }
}
