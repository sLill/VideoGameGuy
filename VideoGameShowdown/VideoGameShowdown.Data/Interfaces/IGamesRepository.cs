namespace VideoGameShowdown.Data
{
    public interface IGamesRepository
    {
        #region Methods..
        Task<Game> GetGameFromRawgIdAsync(int rawgId);

        Task<bool> AddOrUpdateAsync(RawgGame rawgGame);

        Task<bool> AddOrUpdateRangeAsync(IEnumerable<RawgGame> rawgGames);
        #endregion Methods..
    }
}
