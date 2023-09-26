namespace VideoGameGuy.Data
{
    public class IgdbGames_Artworks : ModelBase
    {
        #region Properties..
        public Guid IgdbGames_ArtworksId { get; set; }
        public long Games_SourceId { get; set; }
        public long Artworks_SourceId { get; set; }
        #endregion Properties..

        #region Methods..
        public void Initialize(long games_SourceId, long artworks_SourceId)
        {
            Games_SourceId = games_SourceId;
            Artworks_SourceId = artworks_SourceId;
        }
        #endregion Methods..
    }
}
