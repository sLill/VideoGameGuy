namespace VideoGameGuy.Data
{
    public interface IIgdbGames_ArtworksRepository : IRepositoryBase
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_Artworks> igdbGames_Artworks, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
