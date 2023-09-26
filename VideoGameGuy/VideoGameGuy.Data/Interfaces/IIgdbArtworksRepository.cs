namespace VideoGameGuy.Data
{
    public interface IIgdbArtworksRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiArtwork> apiArtworks, bool suspendSaveChanges = false);
        #endregion Methods..
    }
}
