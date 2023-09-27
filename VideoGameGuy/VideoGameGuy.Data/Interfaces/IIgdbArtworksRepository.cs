namespace VideoGameGuy.Data
{
    public interface IIgdbArtworksRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiArtwork> apiArtworks, bool suspendSaveChanges = false);
        Task<bool> StageBulkChangesAsync(IEnumerable<IgdbApiArtwork> apiArtworks);
        Task<bool> SaveBulkChangesAsync();
        #endregion Methods..
    }
}
