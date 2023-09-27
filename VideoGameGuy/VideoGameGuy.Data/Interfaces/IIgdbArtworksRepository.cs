namespace VideoGameGuy.Data
{
    public interface IIgdbArtworksRepository : IRepositoryBase
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiArtwork> apiArtworks, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
