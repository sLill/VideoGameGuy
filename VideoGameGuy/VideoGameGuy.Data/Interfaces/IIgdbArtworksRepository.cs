namespace VideoGameGuy.Data
{
    public interface IIgdbArtworksRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiArtwork> apiArtworks);
        #endregion Methods..
    }
}
