namespace VideoGameGuy.Data
{
    public class IgdbGames_GameModes : ModelBase
    {
        #region Properties..
        public Guid IgdbGames_GameModesId { get; set; }
        public long Games_SourceId { get; set; }
        public long GameModes_SourceId { get; set; }
        #endregion Properties..

        #region Methods..
        public void Initialize(long games_SourceId, long gameModes_SourceId)
        {
            Games_SourceId = games_SourceId;
            GameModes_SourceId = gameModes_SourceId;
        }
        #endregion Methods..
    }
}
