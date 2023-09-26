namespace VideoGameGuy.Data
{
    public class IgdbGames_MultiplayerModes : ModelBase
    {
        #region Properties..
        public Guid IgdbGames_MultiplayerModesId { get; set; }
        public long Games_SourceId { get; set; }
        public long MultiplayerModes_SourceId { get; set; }
        #endregion Properties..

        #region Methods..
        public void Initialize(long games_SourceId, long multiplayerModes_SourceId)
        {
            Games_SourceId = games_SourceId;
            MultiplayerModes_SourceId = multiplayerModes_SourceId;
        }
        #endregion Methods..
    }
}
