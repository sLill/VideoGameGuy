namespace VideoGameShowdown.Data
{
    public interface IGamesRepository
    {
        #region Methods..
        Task<bool> AddOrUpdateGameAsync(RawgGame rawgGame);
        #endregion Methods..
    }
}
